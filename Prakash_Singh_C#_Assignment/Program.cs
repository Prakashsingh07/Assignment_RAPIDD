using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Prakash_Singh_CSharp_Assignment.Services;

namespace Prakash_Singh_CSharp_Assignment
{
    internal class Program
    {
        private const string DefaultApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

        private static async Task<int> Main(string[] args)
        {
            Console.Title = "RAPIDD Technologies - C# Technical Assessment | Prakash Singh";

            PrintBanner();

            string apiUrl = DefaultApiUrl;
            string outputDir = AppDomain.CurrentDomain.BaseDirectory;

            // Simple CLI arguments parsing if provided
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "--api" || args[i] == "-a") && i + 1 < args.Length)
                {
                    apiUrl = args[++i];
                }
                else if ((args[i] == "--output" || args[i] == "-o") && i + 1 < args.Length)
                {
                    outputDir = args[++i];
                }
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string chartFileName = "employee_time_piechart.png";
            string chartFilePath = Path.Combine(outputDir, chartFileName);

            string htmlReportFileName = "employees_work_summary.html";
            string htmlReportFilePath = Path.Combine(outputDir, htmlReportFileName);

            // Also create index.html in the output directory for quick viewing
            string indexReportFilePath = Path.Combine(outputDir, "index.html");

            try
            {
                // Step 1: Initialize services
                var apiService = new ApiService();
                var processor = new TimeEntryProcessor();
                var chartGenerator = new ChartGenerator();
                var htmlGenerator = new HtmlReportGenerator();

                // Step 2: Retrieve time entries from API
                Console.WriteLine("\n[STEP 1/4] Retrieving time entries from Azure API endpoint...");
                var entries = await apiService.FetchTimeEntriesAsync(apiUrl);

                // Step 3: Process and cleanse data
                Console.WriteLine("\n[STEP 2/4] Processing, aggregating, and ordering employee work hours...");
                var summaries = processor.Process(entries);

                if (summaries.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] No valid employee records found to process.");
                    Console.ResetColor();
                    return 1;
                }

                // Step 4: Generate PNG Pie Chart (Task b)
                Console.WriteLine("\n[STEP 3/4] Generating Task (b) PNG Pie Chart visualization...");
                chartGenerator.GeneratePieChart(summaries, chartFilePath);

                // Step 5: Generate HTML Table Report (Task a)
                Console.WriteLine("\n[STEP 4/4] Generating Task (a) HTML Table page...");
                string htmlContent = htmlGenerator.GenerateHtml(summaries, chartFileName);
                htmlGenerator.SaveHtmlToFile(htmlContent, htmlReportFilePath);
                htmlGenerator.SaveHtmlToFile(htmlContent, indexReportFilePath);

                // Print console summary table
                PrintConsoleTable(summaries);

                // Success summary
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("                    ALL TASKS COMPLETED SUCCESSFULLY!                           ");
                Console.WriteLine("================================================================================");
                Console.ResetColor();
                Console.WriteLine($"  [HTML Report] : {htmlReportFilePath}");
                Console.WriteLine($"  [Pie Chart]   : {chartFilePath}");
                Console.WriteLine("================================================================================\n");

                // Try to open HTML report in default browser
                TryOpenBrowser(htmlReportFilePath);

                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[FATAL ERROR] An unexpected error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return 1;
            }
        }

        private static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"================================================================================");
            Console.WriteLine(@" RAPIDD TECHNOLOGIES - C# TECHNICAL ASSIGNMENT SOLUTION                         ");
            Console.WriteLine(@" Tasks: (a) Visualize JSON via HTML Table | (b) Visualize JSON in PNG Pie Chart ");
            Console.WriteLine(@" Candidate: Prakash Singh | Target: .NET 8                                       ");
            Console.WriteLine(@"================================================================================");
            Console.ResetColor();
        }

        private static void PrintConsoleTable(System.Collections.Generic.IReadOnlyList<Models.EmployeeSummary> summaries)
        {
            Console.WriteLine("\n" + new string('-', 76));
            Console.WriteLine($"{"#",-4} {"Employee Name",-26} {"Total Hours",14} {"% Total",10} {"Status",18}");
            Console.WriteLine(new string('-', 76));

            int rank = 1;
            foreach (var emp in summaries)
            {
                if (emp.IsUnderworked)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{rank,-4} {emp.EmployeeName,-26} {emp.FormattedHours + " hrs",14} {emp.FormattedPercentage,10} {"< 100 hrs (*ALERT*)",18}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{rank,-4} {emp.EmployeeName,-26} {emp.FormattedHours + " hrs",14} {emp.FormattedPercentage,10} {"OK (>= 100 hrs)",18}");
                }
                rank++;
            }
            Console.WriteLine(new string('-', 76));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" (*) Red rows indicate employees who worked less than 100 hours.");
            Console.ResetColor();
        }

        private static void TryOpenBrowser(string filePath)
        {
            try
            {
                if (Environment.UserInteractive && File.Exists(filePath))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
            }
            catch
            {
                // Non-fatal if browser cannot be launched in background/CI
            }
        }
    }
}
