using System.Collections.Generic;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public interface IHtmlReportGenerator
    {
        /// <summary>
        /// Generates a standalone, beautifully styled HTML report displaying an ordered table
        /// of employees with their total time worked and colored rows for employees under 100 hours.
        /// </summary>
        /// <param name="summaries">Ordered list of employee work summaries.</param>
        /// <param name="chartImagePath">Path or relative URL to the generated pie chart PNG.</param>
        /// <returns>HTML document string.</returns>
        string GenerateHtml(IReadOnlyList<EmployeeSummary> summaries, string? chartImagePath = null);

        /// <summary>
        /// Saves the generated HTML report to the specified file path.
        /// </summary>
        void SaveHtmlToFile(string htmlContent, string filePath);
    }
}
