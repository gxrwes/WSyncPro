using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WSyncPro.Core.Test;
using WSyncPro.Core.Test.TestModels;
using Xunit;

namespace WSyncPro.Test.Unit.Core
{
    public class TestReporterUnitTest
    {
        private readonly string TestReportsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReports");

        public TestReporterUnitTest()
        {
            // Ensure TestReports folder is clean before running tests
            if (Directory.Exists(TestReportsFolder))
            {
                Directory.Delete(TestReportsFolder, true);
            }
            Directory.CreateDirectory(TestReportsFolder);
        }

        [Fact]
        public async Task Report_ShouldWriteMultipleJsonFilesWithTagsAndParentSuccessfully()
        {
            // Arrange
            var testReporter = new TestReporter();
            var testReports = GenerateSampleReports();

            // Act
            foreach (var report in testReports)
            {
                var result = await testReporter.Report(report);
                Assert.True(result);

                string expectedFilePath = Path.Combine(TestReportsFolder, $"{report.TestName}_{report.Timestamp}.json");
                Assert.True(File.Exists(expectedFilePath));

                var writtenContent = await File.ReadAllTextAsync(expectedFilePath);
                var writtenReport = JsonSerializer.Deserialize<TestReport>(writtenContent);
                Assert.NotNull(writtenReport);
                Assert.Equal(report.TestName, writtenReport.TestName);
                Assert.Equal(report.Tags, writtenReport.Tags);
                Assert.Equal(report.Parent, writtenReport.Parent);
            }
        }

        [Fact]
        public async Task BuildFinalReport_ShouldGroupTestsByParentAndIncludeTags()
        {
            // Arrange
            var testReporter = new TestReporter();
            var testReports = GenerateSampleReports();

            foreach (var report in testReports)
            {
                await testReporter.Report(report);
            }

            // Act
            var result = await testReporter.BuildFinalReport();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
            Assert.True(File.Exists(result));
            Assert.EndsWith(".html", result);

            var htmlContent = await File.ReadAllTextAsync(result);

            // Verify grouping by parent
            Assert.Contains("Parent: ParentTest1", htmlContent);
            Assert.Contains("Parent: ParentTest2", htmlContent);
            Assert.Contains("Parent: ParentTest3", htmlContent);

            // Verify tags
            Assert.Contains("Integration, Critical", htmlContent);
            Assert.Contains("UnitTest", htmlContent);
            Assert.Contains("Performance", htmlContent);
            Assert.Contains("Smoke, Regression", htmlContent);

            // Verify logs are present
            foreach (var report in testReports)
            {
                Assert.Contains(report.Log, htmlContent);
            }

            // Verify all JSON files were deleted
            var jsonFiles = Directory.GetFiles(TestReportsFolder, "*.json");
            Assert.Empty(jsonFiles);
        }

        [Fact]
        public async Task BuildFinalReport_ShouldHandleNoReportsGracefully()
        {
            // Arrange
            var testReporter = new TestReporter();

            // Ensure the directory is clear before starting the test
            if (Directory.Exists(TestReportsFolder))
            {
                Directory.Delete(TestReportsFolder, true);
            }
            Directory.CreateDirectory(TestReportsFolder);

            // Act
            var result = await testReporter.BuildFinalReport();

            // Assert
            Assert.True(string.IsNullOrEmpty(result), "The method should return an empty string when no test reports are found.");
            var jsonFiles = Directory.GetFiles(TestReportsFolder, "*.json");
            Assert.Empty(jsonFiles);
            Assert.False(Directory.GetFiles(TestReportsFolder, "*.html").Any(), "No HTML files should be generated when no reports are present.");
        }



        [Fact]
        public async Task UpdateReport_ShouldUpdateCorrectJsonFileAndRetainTagsAndParent()
        {
            // Arrange
            var testReporter = new TestReporter();
            var initialReport = new TestReport
            {
                Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                Status = TestStatus.Failure,
                TestName = "TestUpdate",
                Message = "Initial failure message",
                Log = "Initial failure log",
                Parent = "ParentTest",
                Tags = new List<string> { "InitialTag" }
            };

            await testReporter.Report(initialReport);

            // Modify the report
            var updatedReport = new TestReport
            {
                Timestamp = initialReport.Timestamp,
                Status = TestStatus.Success,
                TestName = "TestUpdate",
                Message = "Updated success message",
                Log = "Updated success log",
                Parent = "ParentTest",
                Tags = new List<string> { "UpdatedTag" }
            };

            // Act
            var updateResult = await testReporter.UpdateReport(updatedReport);

            // Assert
            Assert.True(updateResult);

            string filePath = Path.Combine(TestReportsFolder, $"{updatedReport.TestName}_{updatedReport.Timestamp}.json");
            Assert.True(File.Exists(filePath));

            var updatedContent = await File.ReadAllTextAsync(filePath);
            var writtenReport = JsonSerializer.Deserialize<TestReport>(updatedContent);

            Assert.NotNull(writtenReport);
            Assert.Equal("Updated success message", writtenReport.Message);
            Assert.Equal("Updated success log", writtenReport.Log);
            Assert.Equal(TestStatus.Success, writtenReport.Status);
            Assert.Equal("ParentTest", writtenReport.Parent);
            Assert.Contains("UpdatedTag", writtenReport.Tags);
        }

        private List<TestReport> GenerateSampleReports()
        {
            return new List<TestReport>
            {
                new TestReport
                {
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Status = TestStatus.Success,
                    TestName = "ChildTest1",
                    Message = "Child test 1 passed",
                    Log = "Log for child test 1",
                    Parent = "ParentTest1",
                    Tags = new List<string> { "Integration", "Critical" }
                },
                new TestReport
                {
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Status = TestStatus.Failure,
                    TestName = "ChildTest2",
                    Message = "Child test 2 failed",
                    Log = "Log for child test 2",
                    Parent = "ParentTest1",
                    Tags = new List<string> { "UnitTest" }
                },
                new TestReport
                {
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Status = TestStatus.Skipped,
                    TestName = "ChildTest3",
                    Message = "Child test 3 skipped",
                    Log = "Log for child test 3",
                    Parent = "ParentTest2",
                    Tags = new List<string> { "Performance" }
                },
                new TestReport
                {
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Status = TestStatus.Failure,
                    TestName = "SmokeTest",
                    Message = "Smoke test failed",
                    Log = "Log for smoke test",
                    Parent = "ParentTest3",
                    Tags = new List<string> { "Smoke", "Regression" }
                },
                new TestReport
                {
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    Status = TestStatus.Success,
                    TestName = "RegressionTest",
                    Message = "Regression test passed",
                    Log = "Log for regression test",
                    Parent = "ParentTest3",
                    Tags = new List<string> { "Regression" }
                }
            };
        }
    }
}
