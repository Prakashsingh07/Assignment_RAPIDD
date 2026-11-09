using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public class HtmlReportGenerator : IHtmlReportGenerator
    {
        public string GenerateHtml(IReadOnlyList<EmployeeSummary> summaries, string? chartImagePath = null)
        {
            var sb = new StringBuilder();

            double totalHours = summaries.Sum(s => s.TotalHoursWorked);
            int totalEmployees = summaries.Count;
            int underworkedCount = summaries.Count(s => s.IsUnderworked);
            double avgHours = totalEmployees > 0 ? totalHours / totalEmployees : 0;

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Employee Work Time Summary & Analysis | RAPIDD Technologies Assignment</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        :root {");
            sb.AppendLine("            --primary: #1e3a8a;");
            sb.AppendLine("            --primary-light: #3b82f6;");
            sb.AppendLine("            --bg: #f8fafc;");
            sb.AppendLine("            --card-bg: #ffffff;");
            sb.AppendLine("            --text-dark: #0f172a;");
            sb.AppendLine("            --text-muted: #64748b;");
            sb.AppendLine("            --border: #e2e8f0;");
            sb.AppendLine("            --danger-bg: #fee2e2;");
            sb.AppendLine("            --danger-border: #ef4444;");
            sb.AppendLine("            --danger-text: #991b1b;");
            sb.AppendLine("            --success-bg: #dcfce7;");
            sb.AppendLine("            --success-text: #166534;");
            sb.AppendLine("        }");
            sb.AppendLine("        * { box-sizing: border-box; margin: 0; padding: 0; }");
            sb.AppendLine("        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; background-color: var(--bg); color: var(--text-dark); line-height: 1.5; padding: 32px 16px; }");
            sb.AppendLine("        .container { max-width: 1100px; margin: 0 auto; }");
            sb.AppendLine("        header { margin-bottom: 28px; padding-bottom: 20px; border-bottom: 2px solid var(--border); }");
            sb.AppendLine("        h1 { font-size: 28px; font-weight: 700; color: var(--primary); margin-bottom: 6px; }");
            sb.AppendLine("        .subtitle { color: var(--text-muted); font-size: 15px; }");
            sb.AppendLine("        .meta-tag { display: inline-block; background: #e0e7ff; color: #3730a3; padding: 3px 10px; border-radius: 9999px; font-size: 12px; font-weight: 600; margin-left: 8px; }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Metric Cards */");
            sb.AppendLine("        .metrics-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 16px; margin-bottom: 28px; }");
            sb.AppendLine("        .metric-card { background: var(--card-bg); padding: 20px; border-radius: 12px; border: 1px solid var(--border); box-shadow: 0 1px 3px rgba(0,0,0,0.05); }");
            sb.AppendLine("        .metric-label { font-size: 13px; font-weight: 600; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }");
            sb.AppendLine("        .metric-value { font-size: 26px; font-weight: 700; color: var(--text-dark); margin-top: 6px; }");
            sb.AppendLine("        .metric-card.alert { border-color: #fca5a5; background: #fff5f5; }");
            sb.AppendLine("        .metric-card.alert .metric-value { color: var(--danger-text); }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Legend / Note */");
            sb.AppendLine("        .legend-banner { display: flex; align-items: center; gap: 12px; background: #eff6ff; border-left: 4px solid var(--primary-light); padding: 12px 18px; border-radius: 6px; margin-bottom: 24px; font-size: 14px; color: #1e40af; }");
            sb.AppendLine("        .legend-pill { display: inline-block; width: 14px; height: 14px; border-radius: 3px; background-color: var(--danger-bg); border: 1px solid var(--danger-border); vertical-align: middle; margin-right: 4px; }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Main Table */");
            sb.AppendLine("        .table-card { background: var(--card-bg); border-radius: 12px; border: 1px solid var(--border); box-shadow: 0 2px 6px rgba(0,0,0,0.04); overflow: hidden; margin-bottom: 32px; }");
            sb.AppendLine("        .table-header-title { padding: 18px 24px; font-size: 18px; font-weight: 600; border-bottom: 1px solid var(--border); }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; text-align: left; }");
            sb.AppendLine("        thead { background: #f1f5f9; }");
            sb.AppendLine("        th { padding: 14px 20px; font-size: 13px; font-weight: 600; text-transform: uppercase; color: var(--text-muted); letter-spacing: 0.5px; border-bottom: 1px solid var(--border); }");
            sb.AppendLine("        td { padding: 14px 20px; font-size: 14px; border-bottom: 1px solid var(--border); vertical-align: middle; }");
            sb.AppendLine("        tbody tr:hover { background-color: #f8fafc; }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Row Highlight for < 100 hrs (Assignment Requirement) */");
            sb.AppendLine("        tr.underworked-row { background-color: #fee2e2 !important; border-left: 5px solid #ef4444; }");
            sb.AppendLine("        tr.underworked-row:hover { background-color: #fecaca !important; }");
            sb.AppendLine("        tr.underworked-row td { color: #991b1b; font-weight: 500; }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Badges */");
            sb.AppendLine("        .badge { display: inline-block; padding: 4px 10px; font-size: 12px; font-weight: 600; border-radius: 9999px; }");
            sb.AppendLine("        .badge-danger { background-color: #fee2e2; color: #991b1b; border: 1px solid #f87171; }");
            sb.AppendLine("        .badge-success { background-color: var(--success-bg); color: var(--success-text); }");
            sb.AppendLine("        ");
            sb.AppendLine("        /* Chart Section */");
            sb.AppendLine("        .chart-section { background: var(--card-bg); padding: 24px; border-radius: 12px; border: 1px solid var(--border); box-shadow: 0 2px 6px rgba(0,0,0,0.04); text-align: center; }");
            sb.AppendLine("        .chart-section h2 { font-size: 20px; color: var(--primary); margin-bottom: 8px; }");
            sb.AppendLine("        .chart-section p { font-size: 14px; color: var(--text-muted); margin-bottom: 20px; }");
            sb.AppendLine("        .chart-wrapper { display: inline-block; max-width: 100%; border: 1px solid var(--border); border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.06); }");
            sb.AppendLine("        .chart-wrapper img { display: block; max-width: 100%; height: auto; }");
            sb.AppendLine("        ");
            sb.AppendLine("        footer { margin-top: 36px; padding-top: 16px; border-top: 1px solid var(--border); text-align: center; color: var(--text-muted); font-size: 13px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"container\">");
            sb.AppendLine("        <header>");
            sb.AppendLine("            <h1>Employee Work Time Summary</h1>");
            sb.AppendLine($"            <p class=\"subtitle\">RAPIDD Technologies Technical Assessment <span class=\"meta-tag\">Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}</span></p>");
            sb.AppendLine("        </header>");
            sb.AppendLine("");
            sb.AppendLine("        <!-- Executive Metrics -->");
            sb.AppendLine("        <div class=\"metrics-grid\">");
            sb.AppendLine("            <div class=\"metric-card\">");
            sb.AppendLine("                <div class=\"metric-label\">Total Employees</div>");
            sb.AppendLine($"                <div class=\"metric-value\">{totalEmployees}</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"metric-card\">");
            sb.AppendLine("                <div class=\"metric-label\">Total Hours Worked</div>");
            sb.AppendLine($"                <div class=\"metric-value\">{totalHours:F2} hrs</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"metric-card\">");
            sb.AppendLine("                <div class=\"metric-label\">Average Hours / Employee</div>");
            sb.AppendLine($"                <div class=\"metric-value\">{avgHours:F2} hrs</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"metric-card alert\">");
            sb.AppendLine("                <div class=\"metric-label\">Worked &lt; 100 Hours</div>");
            sb.AppendLine($"                <div class=\"metric-value\">{underworkedCount} employee(s)</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("");
            sb.AppendLine("        <!-- Visual Guidance Banner -->");
            sb.AppendLine("        <div class=\"legend-banner\">");
            sb.AppendLine("            <span class=\"legend-pill\"></span>");
            sb.AppendLine("            <span><strong>Note on Table Highlighting:</strong> Rows colored in light red denote employees who worked <strong>less than 100 hours</strong>, as mandated by the assignment specification.</span>");
            sb.AppendLine("        </div>");
            sb.AppendLine("");
            sb.AppendLine("        <!-- Employee Table -->");
            sb.AppendLine("        <div class=\"table-card\">");
            sb.AppendLine("            <div class=\"table-header-title\">Employees Ranked by Total Time Worked</div>");
            sb.AppendLine("            <table>");
            sb.AppendLine("                <thead>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <th style=\"width: 60px;\">Rank</th>");
            sb.AppendLine("                        <th>Employee Name</th>");
            sb.AppendLine("                        <th style=\"text-align: right;\">Total Time Worked (Hours)</th>");
            sb.AppendLine("                        <th style=\"text-align: right;\">Percentage of Total Time</th>");
            sb.AppendLine("                        <th style=\"text-align: center;\">Status</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody>");

            int rank = 1;
            foreach (var emp in summaries)
            {
                string rowClass = emp.IsUnderworked ? "underworked-row" : string.Empty;
                string statusBadge = emp.IsUnderworked
                    ? "<span class=\"badge badge-danger\">Under 100 hrs</span>"
                    : "<span class=\"badge badge-success\">Standard (&ge; 100 hrs)</span>";

                sb.AppendLine($"                    <tr class=\"{rowClass}\">");
                sb.AppendLine($"                        <td><strong>#{rank}</strong></td>");
                sb.AppendLine($"                        <td><strong>{System.Net.WebUtility.HtmlEncode(emp.EmployeeName)}</strong></td>");
                sb.AppendLine($"                        <td style=\"text-align: right; font-variant-numeric: tabular-nums;\">{emp.FormattedHours} hrs</td>");
                sb.AppendLine($"                        <td style=\"text-align: right; font-variant-numeric: tabular-nums;\">{emp.FormattedPercentage}</td>");
                sb.AppendLine($"                        <td style=\"text-align: center;\">{statusBadge}</td>");
                sb.AppendLine("                    </tr>");
                rank++;
            }

            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("        </div>");

            if (!string.IsNullOrEmpty(chartImagePath))
            {
                sb.AppendLine("");
                sb.AppendLine("        <!-- Pie Chart Visualization Section -->");
                sb.AppendLine("        <div class=\"chart-section\">");
                sb.AppendLine("            <h2>Task (b): Visualized Pie Chart</h2>");
                sb.AppendLine("            <p>Percentage of total work hours distributed across each employee</p>");
                sb.AppendLine("            <div class=\"chart-wrapper\">");
                sb.AppendLine($"                <img src=\"{chartImagePath}\" alt=\"Employee Total Time Worked Pie Chart\">");
                sb.AppendLine("            </div>");
                sb.AppendLine("        </div>");
            }

            sb.AppendLine("");
            sb.AppendLine("        <footer>");
            sb.AppendLine("            <p>RAPIDD Technologies C# Technical Assignment Solution &bull; Developed by <strong>Prakash Singh</strong></p>");
            sb.AppendLine("        </footer>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public void SaveHtmlToFile(string htmlContent, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, htmlContent, Encoding.UTF8);
            Console.WriteLine($"[SUCCESS] HTML Table report successfully generated at: {filePath}");
        }
    }
}
