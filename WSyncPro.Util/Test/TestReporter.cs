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
                string filePath = Path.Combine(TestReportsFolder, $"{report.TestName}_{report.TestStepname}_{report.Timestamp.Normalize().Replace(":", "-").Replace(";", "--")}.json");
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
                string filePattern = $"{report.TestName}_{report.TestStepname}_*.json";
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

                var groupedByTestName = reports
                    .GroupBy(r => r.TestName)
                    .OrderBy(g => g.Key);

                string uniqueId = Guid.NewGuid().ToString();
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<html><head>");
                htmlBuilder.Append("<title>Test Report</title>");
                htmlBuilder.Append("<style>");
                htmlBuilder.Append("body { font-family: Arial, sans-serif; line-height: 1.6; background-color: #f4f4f4; padding: 20px; }");
                htmlBuilder.Append("h1, h2, h3 { color: #333; }");
                htmlBuilder.Append(".success { color: green; }");
                htmlBuilder.Append(".failure { color: red; }");
                htmlBuilder.Append(".skipped { color: orange; }");
                htmlBuilder.Append(".tags { font-style: italic; color: #555; }");
                htmlBuilder.Append(".log { background-color: #eee; padding: 10px; border-radius: 5px; }");
                htmlBuilder.Append(".test-step { margin-left: 20px; }");
                htmlBuilder.Append("</style>");
                htmlBuilder.Append("</head><body>");
                htmlBuilder.Append($"<h1>Test Report Overview (ID: {uniqueId})</h1>");

                foreach (var testNameGroup in groupedByTestName)
                {
                    htmlBuilder.Append($"<h2>Test Name: {testNameGroup.Key}</h2>");

                    var groupedByTestStep = testNameGroup
                        .GroupBy(r => r.TestStepname)
                        .OrderBy(g => g.Key);

                    foreach (var testStepGroup in groupedByTestStep)
                    {
                        htmlBuilder.Append($"<h3 class='test-step'>Test Step: {testStepGroup.Key}</h3>");
                        htmlBuilder.Append("<ul>");

                        foreach (var report in testStepGroup)
                        {
                            string statusClass = report.Status.ToString().ToLower();
                            string tags = report.Tags != null ? string.Join(", ", report.Tags) : "No Tags";
                            htmlBuilder.Append($"<li class='{statusClass}'>{report.Timestamp}: {report.Message} <span class='tags'>[{tags}]</span></li>");
                        }

                        htmlBuilder.Append("</ul>");
                    }
                }

                htmlBuilder.Append("<h2>Detailed Logs</h2>");
                foreach (var report in reports)
                {
                    htmlBuilder.Append($"<h3>{report.TestName} - {report.TestStepname} ({report.Timestamp})</h3>");
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