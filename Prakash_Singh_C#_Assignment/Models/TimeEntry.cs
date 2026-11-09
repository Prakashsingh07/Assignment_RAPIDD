using System;
using System.Text.Json.Serialization;

namespace Prakash_Singh_CSharp_Assignment.Models
{
    /// <summary>
    /// Represents a single time entry record retrieved from the Azure API.
    /// </summary>
    public class TimeEntry
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("EmployeeName")]
        public string? EmployeeName { get; set; }

        /// <summary>
        /// Start time in UTC. Note: The API schema names this property "StarTimeUtc".
        /// We also accept "StartTimeUtc" for compatibility.
        /// </summary>
        [JsonPropertyName("StarTimeUtc")]
        public DateTime StarTimeUtc { get; set; }

        [JsonPropertyName("StartTimeUtc")]
        public DateTime? AlternateStartTimeUtc
        {
            get => StarTimeUtc;
            set
            {
                if (value.HasValue && StarTimeUtc == default)
                {
                    StarTimeUtc = value.Value;
                }
            }
        }

        [JsonPropertyName("EndTimeUtc")]
        public DateTime EndTimeUtc { get; set; }

        [JsonPropertyName("EntryNotes")]
        public string? EntryNotes { get; set; }

        [JsonPropertyName("DeletedOn")]
        public DateTime? DeletedOn { get; set; }

        /// <summary>
        /// Duration of the work entry.
        /// </summary>
        [JsonIgnore]
        public TimeSpan Duration => EndTimeUtc - StarTimeUtc;

        /// <summary>
        /// Total hours worked in this entry.
        /// </summary>
        [JsonIgnore]
        public double HoursWorked => Duration.TotalHours;

        /// <summary>
        /// Indicates whether this entry has valid timestamps (EndTime >= StarTime) and has not been deleted.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => DeletedOn == null && EndTimeUtc >= StarTimeUtc;
    }
}
