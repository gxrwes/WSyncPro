using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Util.Test.TestModels;

namespace WSyncPro.Util.Test
{
    public class TestReporter : ITestReporter
    {
        private readonly string TestReportsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReports");

        public TestReporter()
        {
            if (!Directory.Exists(TestReportsFolder))
            {
                Directory.CreateDirectory(TestReportsFolder);
            }
        }

        public async Task<bool> Report(TestReport report)
        {
            try
            {
                string filePath = Path.Combine(TestReportsFolder, $"{report.TestName}_{report.Timestamp}.json");
                string jsonContent = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, jsonContent);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reporting test: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateReport(TestReport report)
        {
            try
            {
                string filePattern = $"{report.TestName}_*.json";
                string[] matchingFiles = Directory.GetFiles(TestReportsFolder, filePattern);

                if (matchingFiles.Length == 0)
                {
                    Console.WriteLine("No matching report found to update.");
                    return false;
                }

                string latestFile = matchingFiles.OrderByDescending(File.GetLastWriteTime).First();
                string jsonContent = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(latestFile, jsonContent);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while updating test report: {ex.Message}");
                return false;
            }
        }

        public async Task<string> BuildFinalReport()
        {
            try
            {
                var files = Directory.GetFiles(TestReportsFolder, "*.json");
                if (files.Length == 0)
                {
                    Console.WriteLine("No test reports found.");
                    return string.Empty;
                }

                var reports = new List<TestReport>();

                foreach (var file in files)
                {
                    string jsonContent = await File.ReadAllTextAsync(file);
                    var report = JsonSerializer.Deserialize<TestReport>(jsonContent);
                    if (report != null)
                    {
                        reports.Add(report);
                    }
                }

                var groupedByParent = reports
                    .GroupBy(r => r.Parent ?? "Ungrouped")
                    .OrderBy(g => g.Key);

                string uniqueId = Guid.NewGuid().ToString();
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<html><head>");
                htmlBuilder.Append("<title>Test Report</title>");
                htmlBuilder.Append("<style>");
                htmlBuilder.Append("body { font-family: Arial, sans-serif; line-height: 1.6; background-color: #f4f4f4; padding: 20px; }");
                htmlBuilder.Append("h1, h2, h3 { color: #333; }");
                htmlBuilder.Append("ul { list-style-type: none; padding: 0; }");
                htmlBuilder.Append("li { padding: 5px; border-bottom: 1px solid #ddd; }");
                htmlBuilder.Append(".success { color: green; }");
                htmlBuilder.Append(".failure { color: red; }");
                htmlBuilder.Append(".skipped { color: orange; }");
                htmlBuilder.Append(".tags { font-style: italic; color: #555; }");
                htmlBuilder.Append(".log { background-color: #eee; padding: 10px; border-radius: 5px; }");
                htmlBuilder.Append("</style>");
                htmlBuilder.Append("</head><body>");
                htmlBuilder.Append($"<h1>Test Report Overview (ID: {uniqueId})</h1>");

                foreach (var parentGroup in groupedByParent)
                {
                    htmlBuilder.Append($"<h2>Parent: {parentGroup.Key}</h2>");
                    var groupedByStatus = parentGroup.GroupBy(r => r.Status);

                    foreach (var statusGroup in groupedByStatus)
                    {
                        string statusClass = statusGroup.Key.ToString().ToLower();
                        htmlBuilder.Append($"<h3 class='{statusClass}'>{statusGroup.Key} Tests: {statusGroup.Count()}</h3>");
                        htmlBuilder.Append("<ul>");
                        foreach (var report in statusGroup)
                        {
                            string tags = report.Tags != null ? string.Join(", ", report.Tags) : "No Tags";
                            htmlBuilder.Append($"<li>{report.TestName} - {report.Message} <span class='tags'>[{tags}]</span></li>");
                        }
                        htmlBuilder.Append("</ul>");
                    }
                }

                htmlBuilder.Append("<h2>Detailed Logs</h2>");
                foreach (var report in reports)
                {
                    htmlBuilder.Append($"<h3>{report.TestName} ({report.Timestamp})</h3>");
                    htmlBuilder.Append($"<p>Status: {report.Status}</p>");
                    htmlBuilder.Append($"<pre class='log'>{report.Log}</pre>");
                }

                htmlBuilder.Append("</body></html>");

                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string outputFilePath = Path.Combine(TestReportsFolder, $"FinalReport_{timestamp}_{uniqueId}.html");

                await File.WriteAllTextAsync(outputFilePath, htmlBuilder.ToString());

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while building final report: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
