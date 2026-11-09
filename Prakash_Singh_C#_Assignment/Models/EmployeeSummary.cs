using System;

namespace Prakash_Singh_CSharp_Assignment.Models
{
    /// <summary>
    /// Aggregated work summary for a single employee.
    /// </summary>
    public class EmployeeSummary
    {
        /// <summary>
        /// Name of the employee.
        /// </summary>
        public string EmployeeName { get; set; } = string.Empty;

        /// <summary>
        /// Total hours worked across all valid entries.
        /// </summary>
        public double TotalHoursWorked { get; set; }

        /// <summary>
        /// Total hours formatted as a rounded integer or 2 decimal places.
        /// </summary>
        public string FormattedHours => $"{TotalHoursWorked:F2}";

        /// <summary>
        /// Percentage of total company work hours completed by this employee.
        /// </summary>
        public double PercentageOfTotal { get; set; }

        /// <summary>
        /// Percentage formatted for display (e.g., "13.81%").
        /// </summary>
        public string FormattedPercentage => $"{PercentageOfTotal:F2}%";

        /// <summary>
        /// Number of valid timesheet entries for this employee.
        /// </summary>
        public int TotalEntries { get; set; }

        /// <summary>
        /// Flag indicating whether the employee worked less than 100 hours.
        /// Rows meeting this condition are highlighted in the report.
        /// </summary>
        public bool IsUnderworked => TotalHoursWorked < 100.0;
    }
}
