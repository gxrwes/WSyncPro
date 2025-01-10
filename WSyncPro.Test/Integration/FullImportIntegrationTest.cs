using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Files;
using WSyncPro.Models.Filter;
using WSyncPro.Models.Import;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WSyncPro.Models.Db;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Settings;
using WSyncPro.Models.Versioning;
using WSyncPro.Test.Mock;
using System.Text.RegularExpressions;

namespace WSyncPro.Test.Integration
{
    [TestFixture]
    public class FullImportIntegrationTest
    {
        private string _srcDir;
        private string _dstDir;
        private string _testFilesPath;
        private ImportService _importService;
        private List<string> _testFiles;
        private static readonly string _tempBaseDir = Path.Combine(Path.GetTempPath(), "TempImportIntegrationTest");
        private ICopyService _copyService;

        [SetUp]
        public void SetUp()
        {
            // Setup base directory
            Directory.CreateDirectory(_tempBaseDir);

            // Setup temporary directories
            _srcDir = Path.Combine(_tempBaseDir, "ImportTest_Src");
            _dstDir = Path.Combine(_tempBaseDir, "ImportTest_Dst");

            Directory.CreateDirectory(_srcDir);
            Directory.CreateDirectory(_dstDir);

            _testFilesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Integration", "test_files.json");

            // Initialize services with NullLogger
            var logger = new NullLogger<ImportService>();
            var copyLogger = new NullLogger<CopyService>();
            var fileVersioning = new MockFileVersioning();
            var appCache = new MockAppCache();

            _copyService = new CopyService(fileVersioning, appCache, copyLogger);
            _importService = new ImportService(logger, _copyService);

            LoadTestFiles();
        }

        private void LoadTestFiles()
        {
            if (File.Exists(_testFilesPath))
            {
                var filesJson = File.ReadAllText(_testFilesPath);
                _testFiles = JsonSerializer.Deserialize<List<string>>(filesJson);
            }
            else
            {
                _testFiles = new List<string>();
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
        public async Task Step2_RunImport()
        {
            // Arrange
            var filterParams = new FilterParams
            {
                Include = new List<string> { "*.gif", "*.mp4" },
                Exclude = new List<string> { "*temp*" },
                FileTypes = new List<string> { ".txt", ".mp4" },
                MaxFileSize = 10000,
                MinFileSize = 1
            };

            var pathBuilder = new List<ImportPathType>
            {
                ImportPathType.DateYear,
                ImportPathType.DateMonth,
                ImportPathType.FileName
            };

            // Act
            var success = await _importService.RunImport(_srcDir, _dstDir, filterParams, pathBuilder);

            // Assert
            Assert.IsTrue(success, "Import process failed.");

            var copiedFiles = Directory.GetFiles(_dstDir, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();
            var cleandCopiedFiles = new List<string>();
            foreach (var copiedFile in copiedFiles) 
            {
                cleandCopiedFiles.Add(GetFileNameFromPath(copiedFile));
            }
            var expectedFiles = _testFiles
                .Where(fileName => filterParams.Include.Any(pattern => fileName.MatchesWildcard(pattern)) &&
                                   !filterParams.Exclude.Any(pattern => fileName.MatchesWildcard(pattern)))
                .ToList();
            var cleanedFilenames = new List<string>();
            foreach(var filename in expectedFiles)
            {
                cleanedFilenames.Add(GetFileNameFromPath(filename));
            }
            foreach (var expectedFile in cleandCopiedFiles)
            {
                Assert.Contains(Path.GetFileName(expectedFile), copiedFiles, $"File {expectedFile} was not copied correctly.");
            }
        }
        public string GetFileNameFromPath(string? filePath)
        {

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            return Path.GetFileName(filePath);
        }

        [Test, Order(3)]
        public void Step3_FinalCleanup()
        {
            TearDown();

            Assert.IsFalse(Directory.Exists(_srcDir), "Source directory was not deleted.");
            Assert.IsFalse(Directory.Exists(_dstDir), "Destination directory was not deleted.");
        }
        
    }
    public static class WildcardExtensions
    {
        public static bool MatchesWildcard(this string input, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
