using System.Collections.Generic;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public interface IChartGenerator
    {
        /// <summary>
        /// Generates a high-quality PNG pie chart displaying the percentage of total time worked by each employee.
        /// </summary>
        /// <param name="summaries">Ordered list of employee work summaries.</param>
        /// <param name="outputFilePath">Destination path for the generated PNG file.</param>
        void GeneratePieChart(IReadOnlyList<EmployeeSummary> summaries, string outputFilePath);
    }
}
