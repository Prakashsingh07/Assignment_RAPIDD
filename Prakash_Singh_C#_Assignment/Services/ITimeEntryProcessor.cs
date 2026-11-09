using System.Collections.Generic;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public interface ITimeEntryProcessor
    {
        /// <summary>
        /// Processes raw time entries, cleanses data anomalies, calculates aggregated total hours
        /// per employee, computes work percentages, and orders employees descending by total time worked.
        /// </summary>
        /// <param name="entries">Raw time entries retrieved from the API.</param>
        /// <returns>Ordered list of employee work summaries.</returns>
        List<EmployeeSummary> Process(IEnumerable<TimeEntry> entries);
    }
}
