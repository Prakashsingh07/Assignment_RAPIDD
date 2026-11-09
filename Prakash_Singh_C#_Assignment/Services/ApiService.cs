using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public ApiService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<List<TimeEntry>> FetchTimeEntriesAsync(string endpointUrl)
        {
            Console.WriteLine($"[INFO] Initiating GET request to API endpoint: {endpointUrl[..Math.Min(60, endpointUrl.Length)]}...");

            try
            {
                using var response = await _httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var entries = JsonSerializer.Deserialize<List<TimeEntry>>(json, JsonOptions);

                if (entries != null && entries.Count > 0)
                {
                    Console.WriteLine($"[SUCCESS] Successfully fetched {entries.Count} time entries from live API.");
                    return entries;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Live API call failed ({ex.Message}). Attempting local fallback...");
                Console.ResetColor();
            }

            // Fallback strategy 1: Check for local timeentries.json in working or execution directory
            var fallbackCandidates = new[]
            {
                "timeentries.json",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "timeentries.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "timeentries.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "timeentries.json")
            };

            foreach (var path in fallbackCandidates)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(path);
                        var entries = JsonSerializer.Deserialize<List<TimeEntry>>(json, JsonOptions);
                        if (entries != null && entries.Count > 0)
                        {
                            Console.WriteLine($"[INFO] Successfully loaded {entries.Count} entries from local fallback: {path}");
                            return entries;
                        }
                    }
                    catch
                    {
                        // Continue to next fallback candidate
                    }
                }
            }

            // Fallback strategy 2: Embedded resource in assembly
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Prakash_Singh_CSharp_Assignment.timeentries.json";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var json = await reader.ReadToEndAsync();
                    var entries = JsonSerializer.Deserialize<List<TimeEntry>>(json, JsonOptions);
                    if (entries != null && entries.Count > 0)
                    {
                        Console.WriteLine($"[INFO] Successfully loaded {entries.Count} entries from embedded fallback dataset.");
                        return entries;
                    }
                }
            }
            catch
            {
                // Ignore embedded resource fallback errors
            }

            throw new InvalidOperationException("Failed to retrieve time entries from API and no valid fallback dataset was found.");
        }
    }
}
