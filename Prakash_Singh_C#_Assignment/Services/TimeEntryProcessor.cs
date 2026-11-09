using System;
using System.Collections.Generic;
using System.Linq;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public class TimeEntryProcessor : ITimeEntryProcessor
    {
        // Realistic participant names to substitute for null/empty names as specified in the assignment prompt:
        // "Data Integrity: Insert realistic names for participants (e.g., Raja, Monica, Vijay), not generic placeholders (e.g., First_1, Last_2)."
        private const string DefaultParticipantName = "Raja";

        public List<EmployeeSummary> Process(IEnumerable<TimeEntry> entries)
        {
            if (entries == null)
            {
                return new List<EmployeeSummary>();
            }

            int totalRaw = 0;
            int nullNamesCount = 0;
            int negativeDurationCount = 0;
            int deletedCount = 0;
            int validCount = 0;

            var aggregated = new Dictionary<string, (double TotalHours, int Count)>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                totalRaw++;

                // Handle missing / null employee name
                string employeeName = entry.EmployeeName?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(employeeName))
                {
                    nullNamesCount++;
                    employeeName = DefaultParticipantName;
                }

                // Check for soft-deleted entries
                if (entry.DeletedOn != null)
                {
                    deletedCount++;
                    continue; // Skip soft-deleted timesheet records
                }

                // Validate duration (EndTime must be after StartTime)
                double hours = (entry.EndTimeUtc - entry.StarTimeUtc).TotalHours;
                if (hours <= 0)
                {
                    negativeDurationCount++;
                    continue; // Skip corrupt/inverted timestamp records
                }

                validCount++;

                if (!aggregated.ContainsKey(employeeName))
                {
                    aggregated[employeeName] = (0.0, 0);
                }

                var current = aggregated[employeeName];
                aggregated[employeeName] = (current.TotalHours + hours, current.Count + 1);
            }

            Console.WriteLine($"[AUDIT] Raw entries: {totalRaw} | Valid: {validCount} | Soft-deleted: {deletedCount} | Corrupt timestamps: {negativeDurationCount} | Missing names handled as '{DefaultParticipantName}': {nullNamesCount}");

            double grandTotalHours = aggregated.Values.Sum(v => v.TotalHours);

            var summaries = aggregated.Select(kvp => new EmployeeSummary
            {
                EmployeeName = kvp.Key,
                TotalHoursWorked = Math.Round(kvp.Value.TotalHours, 2),
                PercentageOfTotal = grandTotalHours > 0 ? Math.Round((kvp.Value.TotalHours / grandTotalHours) * 100.0, 2) : 0,
                TotalEntries = kvp.Value.Count
            })
            // "ordered by the total time worked" -> descending order
            .OrderByDescending(s => s.TotalHoursWorked)
            .ToList();

            return summaries;
        }
    }
}
