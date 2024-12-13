using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using WSyncPro.Util.Test;
using WSyncPro.Util.Test.TestModels;

namespace WSyncPro.Test.Integration.Test
{
    public class TestReporterIntegrationTest
    {
        private readonly TestReporter _reporter;

        public TestReporterIntegrationTest()
        {
            string reportsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReports");
            if (Directory.Exists(reportsFolder))
            {
                Directory.Delete(reportsFolder, true);
            }
            _reporter = new TestReporter();
        }

        [Fact]
        public void TestSetup()
        {
            var setupReport = new TestReport
            {
                TestName = "IntegrationTest",
                TestStepname = "Setup",
                Status = TestStatus.Success,
                Message = "Test setup completed successfully.",
                Log = "Setup log details here.",
                Tags = new List<string> { "Setup" }
            };

            bool result = _reporter.Report(setupReport).GetAwaiter().GetResult();
            Assert.True(result, "Failed to report setup step.");
        }

        [Fact]
        public void TestStep1()
        {
            var step1Report = new TestReport
            {
                TestName = "IntegrationTest",
                TestStepname = "Step1",
                Status = TestStatus.Success,
                Message = "Step 1 executed successfully.",
                Log = "Step 1 log details here.",
                Tags = new List<string> { "Execution" }
            };

            bool result = _reporter.Report(step1Report).GetAwaiter().GetResult();
            Assert.True(result, "Failed to report step 1.");
        }

        [Fact]
        public void TestStep2()
        {
            var step2Report = new TestReport
            {
                TestName = "IntegrationTest",
                TestStepname = "Step2",
                Status = TestStatus.Failure,
                Message = "Step 2 failed due to an error.",
                Log = "Step 2 log details with error details here.",
                Tags = new List<string> { "Execution", "Failure" }
            };

            bool result = _reporter.Report(step2Report).GetAwaiter().GetResult();
            Assert.True(result, "Failed to report step 2.");
        }

        [Fact]
        public void TestStep3()
        {
            var step3Report = new TestReport
            {
                TestName = "IntegrationTest",
                TestStepname = "Step3",
                Status = TestStatus.Success,
                Message = "Step 3 executed successfully.",
                Log = "Step 3 log details here.",
                Tags = new List<string> { "Execution" }
            };

            bool result = _reporter.Report(step3Report).GetAwaiter().GetResult();
            Assert.True(result, "Failed to report step 3.");
        }

        [Fact]
        public void CleanupAndGenerateReport()
        {
            var cleanupReport = new TestReport
            {
                TestName = "IntegrationTest",
                TestStepname = "Cleanup",
                Status = TestStatus.Success,
                Message = "Test cleanup completed successfully.",
                Log = "Cleanup log details here.",
                Tags = new List<string> { "Cleanup" }
            };

            bool cleanupResult = _reporter.Report(cleanupReport).GetAwaiter().GetResult();
            Assert.True(cleanupResult, "Failed to report cleanup step.");

            // Generate Final Report
            string finalReportPath = _reporter.BuildFinalReport().GetAwaiter().GetResult();
            Assert.False(string.IsNullOrEmpty(finalReportPath), "Final report path is empty.");
            Assert.True(File.Exists(finalReportPath), "Final report file does not exist.");
        }
    }
}