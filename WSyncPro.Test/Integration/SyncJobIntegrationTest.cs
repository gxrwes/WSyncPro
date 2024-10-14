using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Enum;
using WSyncPro.Util.Files;
using WSyncPro.Util.Services;
using Xunit;

namespace WSyncPro.Test.Integration
{
    public class SyncJobIntegrationTest
    {
        private readonly string testDirectory = ".\\TestDirectory"; // Relative to project directory
        private readonly string srcDirectory = "src";
        private readonly string dstDirectory = "dst";
        private readonly string tempDirectory = Path.Combine(AppContext.BaseDirectory, "TempReports"); // Directory for temp reports
        private readonly string reportFilePath = Path.Combine(AppContext.BaseDirectory, "VerifiedWorkingFiltersReport.md");

        // Extended range of file types for thorough testing
        private readonly string[] testFiles = new[]
        {
            "test1.mp3", "test2.txt", "test3.bin", "test4.png", "test5.mp4", "test6.avi",
            "test7.cs", "test8.js", "test9.html", "test10.css", "test11.pdf", "test12.xml",
            "test13.docx", "test14.zip", "test15.wav", "test16.mkv", "test17.cpp", "test18.py",
            "test_audio.mp3", "test_script.sh", "test_archive.tar.gz", "test_backup.bak",
            "readme.md", "test_video.mpg", "notes.txt", "photo.jpeg"
        };

        private readonly string[] testFilters = new[]
        {
            "*.mp3", "*.txt", "*.bin", "*.png", "*.mp4", "*.avi", "*.cs", "*.js", "*.html", "*.css",
            "*.pdf", "*.xml", "*.docx", "*.zip", "*.wav", "*.mkv", "*.cpp", "*.py", "*.sh", "*.tar.gz",
            "*.bak", "*.md", "*.mpg", "*.jpeg", "test*.mp3", "*_backup.bak", "test_archive*", "*.doc?", "*.cs?", "*script.sh"
        };

        private SyncService _syncService;
        private ILogger<SyncService> _logger;

        public SyncJobIntegrationTest()
        {
            SetupSyncService();
        }

        private void SetupSyncService()
        {
            var fileLoader = new FileLoader();
            var directoryScannerService = new DirectoryScannerService();
            var fileCopyMoveService = new FileCopyMoveService();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                });
            });

            _logger = loggerFactory.CreateLogger<SyncService>();
            _syncService = new SyncService(fileLoader, directoryScannerService, fileCopyMoveService, _logger);
        }

        private void SetupTestEnvironment()
        {
            string srcPath = Path.Combine(testDirectory, srcDirectory);
            string dstPath = Path.Combine(testDirectory, dstDirectory);

            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true); // Clean up any previous test data
            }

            Directory.CreateDirectory(srcPath);
            Directory.CreateDirectory(dstPath);

            // Create sample files in the src directory
            foreach (var fileName in testFiles)
            {
                File.WriteAllText(Path.Combine(srcPath, fileName), "Sample content");
            }

            // Ensure temp directory exists
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
        }

        [Theory]
        [Trait("Category", "Integration")]
        [MemberData(nameof(FilterTestData))]
        public async Task Test_GenericFilters(string filter, bool isInclude, bool combined)
        {
            SetupTestEnvironment();

            var includeFilters = isInclude ? new List<string> { filter } : new List<string>();
            var excludeFilters = isInclude ? new List<string>() : new List<string> { filter };

            // Combined mode adds a secondary filter to test inclusion/exclusion combinations
            if (combined)
            {
                includeFilters.Add("*.mp3");
                excludeFilters.Add("*.txt");
            }

            var job = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = $"Test {filter} {(isInclude ? "Include" : "Exclude")}",
                Description = $"Test {(isInclude ? "including" : "excluding")} files with filter '{filter}'",
                SrcDirectory = Path.Combine(testDirectory, srcDirectory),
                DstDirectory = Path.Combine(testDirectory, dstDirectory),
                FilterInclude = includeFilters,
                FilterExclude = excludeFilters,
                Enabled = true,
                Status = Status.Running
            };

            await _syncService.AddJob(job);
            await RunAndVerifyJob(job, dstFiles =>
            {
                // Example assertions (add specific logic per filter as needed)
                if (isInclude)
                {
                    Assert.All(dstFiles, file => Assert.Contains(filter.TrimStart('*'), file));
                }
                else
                {
                    Assert.All(dstFiles, file => Assert.DoesNotContain(filter.TrimStart('*'), file));
                }
            });

            // Remove job after run
            _syncService = new SyncService(new FileLoader(), new DirectoryScannerService(), new FileCopyMoveService(), _logger); // Recreate instance

            // Clean destination directory after each job
            CleanupDestinationDirectory();
        }

        private async Task RunAndVerifyJob(SyncJob job, Action<string[]> verifyAction)
        {
            var result = await _syncService.RunAllEnabledJobs();

            // Fail the test if no files were processed
            Assert.True(result.filesProcessed > 0, $"Job '{job.Name}' failed to process files.");

            // Verify the expected result for the job
            string[] dstFiles = Directory.GetFiles(Path.Combine(testDirectory, dstDirectory));
            verifyAction(dstFiles);

            // Clean up after each test
            SaveJobResultsToTempFile(job, result.filesProcessed);
        }

        private void SaveJobResultsToTempFile(SyncJob job, int filesProcessed)
        {
            // Save job results to a temporary file
            string tempFilePath = Path.Combine(tempDirectory, $"temp_{job.Id}.md");
            using (var writer = new StreamWriter(tempFilePath))
            {
                writer.WriteLine($"## Job Name: {job.Name}");
                writer.WriteLine($"- Description: {job.Description}");
                writer.WriteLine($"- Filters Included: {string.Join(", ", job.FilterInclude)}");
                writer.WriteLine($"- Filters Excluded: {string.Join(", ", job.FilterExclude)}");
                writer.WriteLine($"- Files Processed: {filesProcessed}");
                writer.WriteLine();
            }
        }

        private void CleanupDestinationDirectory()
        {
            var dstPath = Path.Combine(testDirectory, dstDirectory);
            if (Directory.Exists(dstPath))
            {
                Directory.Delete(dstPath, true);
            }

            Directory.CreateDirectory(dstPath); // Recreate the empty destination folder
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GenerateVerificationReport()
        {
            // Combine all temporary files into the final report
            using (var writer = new StreamWriter(reportFilePath))
            {
                bool headerWritten = false;

                foreach (var tempFile in Directory.GetFiles(tempDirectory, "temp_*.md"))
                {
                    if (!headerWritten)
                    {
                        writer.WriteLine("# Verified Working Filters Report");
                        writer.WriteLine($"## Final Report generated on: {DateTime.Now}");
                        writer.WriteLine();
                        headerWritten = true;
                    }

                    using (var reader = new StreamReader(tempFile))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
            }

            // Clean up temporary files
            CleanupTempFiles();
        }

        private void CleanupTempFiles()
        {
            foreach (var tempFile in Directory.GetFiles(tempDirectory, "temp_*.md"))
            {
                File.Delete(tempFile);
            }
        }

        public static IEnumerable<object[]> FilterTestData()
        {
            var testFilters = new[]
            {
                "*.mp3", "*.txt", "*.bin", "*.png", "*.mp4", "*.avi", "*.cs", "*.js", "*.html", "*.css",
                "*.pdf", "*.xml", "*.docx", "*.zip", "*.wav", "*.mkv", "*.cpp", "*.py", "*.sh", "*.tar.gz",
                "*.bak", "*.md", "*.mpg", "*.jpeg", "test*.mp3", "*_backup.bak", "test_archive*", "*.doc?", "*.cs?", "*script.sh"
            };

            foreach (var filter in testFilters)
            {
                yield return new object[] { filter, true, false };   // Include single filter
                yield return new object[] { filter, false, false };  // Exclude single filter
                yield return new object[] { filter, true, true };    // Include combined with other filters
                yield return new object[] { filter, false, true };   // Exclude combined with other filters
            }
        }
    }
}
