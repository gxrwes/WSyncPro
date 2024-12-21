using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WSyncPro.Core.Services;
using WSyncPro.Models.Filter;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using Microsoft.Extensions.Logging.Abstractions;
using WSyncPro.Util.Test.TestModels;
using WSyncPro.Util.Test;

namespace WSyncPro.Test.Integration
{
    [TestFixture]
    public class FullSyncIntegrationTest
    {
        private string _srcDir;
        private string _dstDir;
        private string _tempDbFile;
        private SyncService _syncService;
        private ICopyService _copyService;
        private IAppCache _appCache;
        private IAppLocalDb _localDb;
        private ITestReporter _testReporter;
        private List<FilterParams> _filterConfigs;
        private List<string> _testFiles;
        private List<SyncJob> _testJobs;
        private static readonly string _tempBaseDir = Path.Combine(Path.GetTempPath(), "TempIntegrationTest");
        private static readonly string _tempUtilDir = Path.Combine(_tempBaseDir, "TempTestUtilFiles");

        [SetUp]
        public void SetUp()
        {
            // Setup base directory
            Directory.CreateDirectory(_tempBaseDir);

            // Setup temporary directories
            _srcDir = Path.Combine(_tempBaseDir, "SyncTest_Src");
            _dstDir = Path.Combine(_tempBaseDir, "SyncTest_Dst");
            _tempDbFile = Path.Combine(_tempBaseDir, "SyncTest_Db.json");

            Directory.CreateDirectory(_srcDir);
            Directory.CreateDirectory(_dstDir);
            Directory.CreateDirectory(_tempUtilDir);

            // Initialize services with NullLogger
            var loggerFactory = LoggerFactory.Create(builder => { }); // No logging
            _localDb = new AppLocalDb(_tempDbFile, NullLogger<AppLocalDb>.Instance);
            _appCache = new AppCache(_localDb, NullLogger<AppCache>.Instance);
            _copyService = new CopyService(new FileVersioning(), _appCache, NullLogger<CopyService>.Instance);
            _syncService = new SyncService(_appCache, _copyService, new FileVersioning(), NullLogger<SyncService>.Instance);
            _testReporter = new TestReporter();

            // Load configuration files
            LoadTestConfigurations();
            LoadTestJobs();

            // Ensure all test jobs use test directories
            ReplaceTestJobDirectories();
        }

        private void LoadTestConfigurations()
        {
            // Load filter configurations
            var filterConfigPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_filters.json");
            if (File.Exists(filterConfigPath))
            {
                var filterJson = File.ReadAllText(filterConfigPath);
                _filterConfigs = JsonSerializer.Deserialize<List<FilterParams>>(filterJson);
            }
            else
            {
                _filterConfigs = new List<FilterParams>();
            }

            // Load test files configuration
            var testFilesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_files.json");
            if (File.Exists(testFilesPath))
            {
                var filesJson = File.ReadAllText(testFilesPath);
                _testFiles = JsonSerializer.Deserialize<List<string>>(filesJson);
            }
            else
            {
                _testFiles = new List<string>();
            }
        }

        private void LoadTestJobs()
        {
            // Load test jobs configuration
            var testJobsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_jobs.json");
            if (File.Exists(testJobsPath))
            {
                var jobsJson = File.ReadAllText(testJobsPath);
                _testJobs = JsonSerializer.Deserialize<List<SyncJob>>(jobsJson);
            }
            else
            {
                _testJobs = new List<SyncJob>();
            }
        }

        private void ReplaceTestJobDirectories()
        {
            foreach (var job in _testJobs)
            {
                job.SrcDirectory = _srcDir;
                job.DstDirectory = _dstDir;
            }
        }

        public void TearDown()
        {
            // Clean up temporary directories and files
            if (Directory.Exists(_tempBaseDir))
                Directory.Delete(_tempBaseDir, true);
        }

        [Test, Order(1)]
        public void Step1_CreateTestFiles()
        {
            foreach (var file in _testFiles)
            {
                var filePath = Path.Combine(_srcDir, file);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directories exist
                File.WriteAllText(filePath, $"Test content for {file}");
            }

            Assert.IsTrue(Directory.GetFiles(_srcDir, "*", SearchOption.AllDirectories).Length > 0, "No test files were created.");
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(GetSyncJobs))]
        public async Task Step2_RunSyncJob(SyncJob job)
        {
            job.SrcDirectory = _srcDir;
            job.DstDirectory = _dstDir;
            Assert.NotNull(job, "Sync job was not created.");
            Step2_4_CleanupDst();
            Step2_1_CreateSyncJob(job);
            await Step2_2_RunSyncJob(job);
            Step2_3_ValidateSyncJob(job);
            
        }

        private void Step2_1_CreateSyncJob(SyncJob job)
        {
            var jobFilePath = Path.Combine(_tempUtilDir, $"SyncJob_{Guid.NewGuid()}.json");
            File.WriteAllText(jobFilePath, JsonSerializer.Serialize(job));
        }

        private async Task Step2_2_RunSyncJob(SyncJob job)
        {
            job.SrcDirectory = _srcDir;
            job.DstDirectory = _dstDir;

            var copyJobs = await _syncService.CreateCpJobsForSyncJob(job);
            Assert.IsNotEmpty(copyJobs, "No copy jobs were created.");

            var executionResults = await _syncService.ExecuteAndVerifyJobs(copyJobs);
            Assert.IsTrue(executionResults.All(r => r), "Not all copy jobs were executed successfully.");
        }

        private void Step2_3_ValidateSyncJob(SyncJob job)
        {
            var copiedFiles = Directory.GetFiles(_dstDir, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();
            bool filefailed = false;
            string jobid = string.Empty;

            jobid = job.Id.ToString();
            foreach (var filterParams in _filterConfigs)
            {
                foreach (var file in _testFiles)
                {
                    var fileName = Path.GetFileName(file);

                    var shouldBeIncluded = filterParams.Include.Any(include => fileName.Contains(include)) &&
                                           !filterParams.Exclude.Any(exclude => fileName.Contains(exclude));
                    try
                    {

                        if (shouldBeIncluded)
                        {
                            Assert.Contains(fileName, copiedFiles, $"File {fileName} was not copied but should have been.");
                        }
                        else
                        {
                            Assert.IsFalse(copiedFiles.Contains(fileName), $"File {fileName} was copied but should not have been.");
                        }
                        _testReporter.Report(new TestReport(TestStatus.Success, "FullSyncIntegrationTest", "Step2_3_ValidateSyncJob_" + jobid, "Succeded for Params: " + job.FilterParams.ToString() + "For File " + file)).GetAwaiter().GetResult(); 

                    }
                    catch (Exception ex) 
                    {
                        _testReporter.Report(new TestReport(TestStatus.Failure, "FullSyncIntegrationTest", "Step2_3_ValidateSyncJob_" + jobid, "Failed for Params: " + job.FilterParams.ToString() + " with error: "+ ex.Message)).GetAwaiter().GetResult();
                        filefailed = true;
                    }
                }
            }
            Assert.That(filefailed, Is.False,"A File has failed -> check testreport");
        }

        private void Step2_4_CleanupDst()
        {
            foreach (var file in Directory.GetFiles(_dstDir, "*", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }

            foreach (var dir in Directory.GetDirectories(_dstDir, "*", SearchOption.AllDirectories))
            {
                Directory.Delete(dir, true);
            }
        }

        [Test, Order(3)]
        public void Step3_FinalCleanup()
        {
            TearDown();
            try
            {
                Assert.IsFalse(Directory.Exists(_srcDir), "Source directory was not deleted.");
                Assert.IsFalse(Directory.Exists(_dstDir), "Destination directory was not deleted.");
                _testReporter.Report(new TestReport(TestStatus.Success, "FullSyncIntegrationTest", "Step3_FinalCleanup", ""));
            }
            catch (Exception ex)
            {
                _testReporter.Report(new TestReport(TestStatus.Failure, "FullSyncIntegrationTest", "Step3_FinalCleanup", "Failed with " + ex.Message));
            }
            _testReporter.BuildFinalReport().GetAwaiter().GetResult();
        }


        private static IEnumerable<SyncJob> GetSyncJobs()
        {
            var testJobsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_jobs.json");
            if (File.Exists(testJobsPath))
            {
                var jobsJson = File.ReadAllText(testJobsPath);
                var jobs = JsonSerializer.Deserialize<List<SyncJob>>(jobsJson) ?? new List<SyncJob>();
                return jobs;
            }

            return Enumerable.Empty<SyncJob>();
        }

        private static IEnumerable<FilterParams> GetFilterConfigs()
        {
            var filterConfigPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_filters.json");
            if (File.Exists(filterConfigPath))
            {
                var filterJson = File.ReadAllText(filterConfigPath);
                return JsonSerializer.Deserialize<List<FilterParams>>(filterJson);
            }

            return new List<FilterParams>();
        }
    }
}
