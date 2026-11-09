using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public class ChartGenerator : IChartGenerator
    {
        // Distinct, accessible, professional color palette for pie chart slices
        private static readonly Color[] Palette = new[]
        {
            Color.FromArgb(37, 99, 235),   // Royal Blue
            Color.FromArgb(16, 185, 129),  // Emerald Green
            Color.FromArgb(245, 158, 11),  // Amber / Warm Gold
            Color.FromArgb(139, 92, 246),  // Purple / Violet
            Color.FromArgb(236, 72, 153),  // Pink
            Color.FromArgb(6, 182, 212),   // Cyan / Sky
            Color.FromArgb(249, 115, 22),  // Orange
            Color.FromArgb(99, 102, 241),  // Indigo
            Color.FromArgb(132, 204, 22),  // Lime
            Color.FromArgb(244, 63, 94),   // Rose
            Color.FromArgb(20, 184, 166),  // Teal
            Color.FromArgb(168, 85, 247),  // Fuchsia
            Color.FromArgb(100, 116, 139)  // Slate
        };

        public void GeneratePieChart(IReadOnlyList<EmployeeSummary> summaries, string outputFilePath)
        {
            if (summaries == null || summaries.Count == 0)
            {
                throw new ArgumentException("Summaries collection cannot be null or empty.", nameof(summaries));
            }

            const int width = 1100;
            const int height = 720;

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bitmap);

            // Configure maximum rendering quality
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Background canvas
            using (var bgBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
            {
                g.FillRectangle(bgBrush, 0, 0, width, height);
            }

            // Top Header Banner
            using (var headerBrush = new SolidBrush(Color.FromArgb(30, 58, 138)))
            using (var titleFont = new Font("Segoe UI", 22, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var subFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var subBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                g.DrawString("Employee Work Hours Distribution", titleFont, headerBrush, 40, 24);
                g.DrawString("Percentage of Total Time Worked (RAPIDD Technologies Technical Assessment)", subFont, subBrush, 40, 56);
            }

            // Separator Line
            using (var linePen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
            {
                g.DrawLine(linePen, 40, 85, width - 40, 85);
            }

            // Pie Geometry
            const int pieX = 50;
            const int pieY = 130;
            const int pieDiameter = 480;
            var pieRect = new Rectangle(pieX, pieY, pieDiameter, pieDiameter);
            var center = new PointF(pieX + pieDiameter / 2.0f, pieY + pieDiameter / 2.0f);
            float radius = pieDiameter / 2.0f;

            double totalHours = summaries.Sum(s => s.TotalHoursWorked);
            float currentAngle = 0.0f;

            using var slicePen = new Pen(Color.White, 2.5f);
            using var labelFont = new Font("Segoe UI", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            using var textShadowBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            using var textWhiteBrush = new SolidBrush(Color.White);
            using var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // First pass: Draw slices
            for (int i = 0; i < summaries.Count; i++)
            {
                var emp = summaries[i];
                float sweepAngle = totalHours > 0
                    ? (float)((emp.TotalHoursWorked / totalHours) * 360.0)
                    : 360.0f / summaries.Count;

                var color = Palette[i % Palette.Length];
                using (var brush = new SolidBrush(color))
                {
                    g.FillPie(brush, pieRect, currentAngle, sweepAngle);
                }

                // Draw clean white border
                g.DrawPie(slicePen, pieRect, currentAngle, sweepAngle);

                // Draw percentage label inside slice if large enough (>= 4% of total)
                if (emp.PercentageOfTotal >= 4.0)
                {
                    double midAngleDeg = currentAngle + (sweepAngle / 2.0);
                    double midAngleRad = midAngleDeg * Math.PI / 180.0;

                    // Position label at 68% radius
                    float labelR = radius * 0.68f;
                    float lx = center.X + (float)(labelR * Math.Cos(midAngleRad));
                    float ly = center.Y + (float)(labelR * Math.Sin(midAngleRad));

                    string labelText = $"{emp.PercentageOfTotal:F1}%";

                    // Shadow for high legibility
                    g.DrawString(labelText, labelFont, textShadowBrush, lx + 1, ly + 1, stringFormat);
                    g.DrawString(labelText, labelFont, textWhiteBrush, lx, ly, stringFormat);
                }

                currentAngle += sweepAngle;
            }

            // Legend Panel on the right
            int legendX = 580;
            int legendY = 120;
            int legendWidth = 480;
            int legendHeight = height - 190;

            // Legend container card
            using (var legendBgBrush = new SolidBrush(Color.White))
            using (var legendBorderPen = new Pen(Color.FromArgb(226, 232, 240), 1.0f))
            {
                var legendRect = new Rectangle(legendX, legendY, legendWidth, legendHeight);
                g.FillRectangle(legendBgBrush, legendRect);
                g.DrawRectangle(legendBorderPen, legendRect);
            }

            // Legend Title
            using (var legendTitleFont = new Font("Segoe UI", 14, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var legendTitleBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            using (var legendSubFont = new Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var legendMutedBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                g.DrawString("Employee Share of Total Hours", legendTitleFont, legendTitleBrush, legendX + 20, legendY + 16);
                g.DrawString($"Total Logged Work: {totalHours:F2} hrs ({summaries.Count} Employees)", legendSubFont, legendMutedBrush, legendX + 20, legendY + 38);
            }

            // Legend Rows
            int itemStartY = legendY + 68;
            int rowHeight = (legendHeight - 80) / Math.Max(1, summaries.Count);
            if (rowHeight > 38) rowHeight = 38;

            using (var nameFont = new Font("Segoe UI", 12, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var nameBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (var statFont = new Font("Segoe UI", 12, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var statBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            using (var alertFont = new Font("Segoe UI", 10, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var alertBgBrush = new SolidBrush(Color.FromArgb(254, 226, 226)))
            using (var alertTextBrush = new SolidBrush(Color.FromArgb(185, 28, 28)))
            {
                for (int i = 0; i < summaries.Count; i++)
                {
                    var emp = summaries[i];
                    var color = Palette[i % Palette.Length];
                    int itemY = itemStartY + (i * rowHeight);

                    // Color swatch pill
                    using (var swatchBrush = new SolidBrush(color))
                    {
                        g.FillRectangle(swatchBrush, legendX + 20, itemY + 2, 16, 16);
                    }
                    using (var swatchPen = new Pen(Color.FromArgb(200, 200, 200), 1.0f))
                    {
                        g.DrawRectangle(swatchPen, legendX + 20, itemY + 2, 16, 16);
                    }

                    // Employee Name
                    g.DrawString(emp.EmployeeName, nameFont, nameBrush, legendX + 46, itemY + 1);

                    // Hours & Percentage
                    string statText = $"{emp.FormattedHours} hrs ({emp.FormattedPercentage})";
                    g.DrawString(statText, statFont, statBrush, legendX + 230, itemY + 1);

                    // Underworked tag (< 100 hrs)
                    if (emp.IsUnderworked)
                    {
                        var badgeRect = new Rectangle(legendX + 395, itemY, 65, 18);
                        g.FillRectangle(alertBgBrush, badgeRect);
                        using (var borderPen = new Pen(Color.FromArgb(248, 113, 113), 1.0f))
                        {
                            g.DrawRectangle(borderPen, badgeRect);
                        }
                        g.DrawString("< 100h", alertFont, alertTextBrush, legendX + 406, itemY + 2);
                    }
                }
            }

            // Footer / Metadata
            using (var footerFont = new Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var footerBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                string footerText = $"RAPIDD Technologies Technical Assessment | Generated on {DateTime.Now:dd-MMM-yyyy HH:mm} | Candidate: Prakash Singh";
                g.DrawString(footerText, footerFont, footerBrush, 40, height - 32);
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bitmap.Save(outputFilePath, ImageFormat.Png);
            Console.WriteLine($"[SUCCESS] Pie Chart image successfully saved to: {outputFilePath}");
        }
    }
}
