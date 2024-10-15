using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using Xunit;

namespace WSyncPro.Test.Unit.Core.Services
{
    public class DirectoryScannerServiceUnitTest : IDisposable
    {
        private readonly string _testDirectory;
        private readonly DirectoryScannerService _service;

        public DirectoryScannerServiceUnitTest()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Create test files and directories
            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "Test File 1");
            File.WriteAllText(Path.Combine(_testDirectory, "file2.log"), "Test File 2");

            var subdirPath = Path.Combine(_testDirectory, "subdir");
            Directory.CreateDirectory(subdirPath);
            File.WriteAllText(Path.Combine(subdirPath, "file3.txt"), "Test File 3");

            _service = new DirectoryScannerService();
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task ScanAsync_ShouldScanDirectoryWithoutFilters()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                SrcDirectory = _testDirectory,
                FilterInclude = new List<string>(),
                FilterExclude = new List<string>()
            };

            // Act
            var result = await _service.ScanAsync(syncJob);

            // Assert
            Assert.Equal(3, result.Count); // Should find 3 files

            // Verify that the RelativePath is correctly set
            var file1 = result.OfType<WFile>().FirstOrDefault(f => f.Name == "file1.txt");
            var file2 = result.OfType<WFile>().FirstOrDefault(f => f.Name == "file2.log");
            var file3 = result.OfType<WFile>().FirstOrDefault(f => f.Name == "file3.txt");

            Assert.NotNull(file1);
            Assert.Equal("file1.txt".Replace("\\", "/"), file1.RelativePath.Replace("\\", "/"));

            Assert.NotNull(file2);
            Assert.Equal("file2.log".Replace("\\", "/"), file2.RelativePath.Replace("\\", "/"));

            Assert.NotNull(file3);
            Assert.Equal("subdir/file3.txt".Replace("\\", "/"), file3.RelativePath.Replace("\\", "/"));
        }


        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task ScanAsync_ShouldSetCorrectRelativePaths()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                SrcDirectory = _testDirectory,
                FilterInclude = new List<string>(),
                FilterExclude = new List<string>()
            };

            // Act
            var result = await _service.ScanAsync(syncJob);

            // Assert
            foreach (var wObject in result)
            {
                if (wObject is WFile wFile)
                {
                    var expectedRelativePath = Path.GetRelativePath(syncJob.SrcDirectory, wFile.FullPath);
                    Assert.Equal(expectedRelativePath.Replace("\\", "/"), wFile.RelativePath.Replace("\\", "/"));
                }
            }
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task ScanAsync_ShouldApplyIncludeFilterCorrectly()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                SrcDirectory = _testDirectory,
                FilterInclude = new List<string> { "*.txt" },
                FilterExclude = new List<string>()
            };

            // Act
            var result = await _service.ScanAsync(syncJob);

            // Assert
            Assert.Equal(2, result.Count); // Should only include .txt files
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task ScanAsync_ShouldApplyExcludeFilterCorrectly()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                SrcDirectory = _testDirectory,
                FilterInclude = new List<string>(),
                FilterExclude = new List<string> { "*.log" }
            };

            // Act
            var result = await _service.ScanAsync(syncJob);

            // Assert
            Assert.Equal(2, result.Count); // Should exclude .log files
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
