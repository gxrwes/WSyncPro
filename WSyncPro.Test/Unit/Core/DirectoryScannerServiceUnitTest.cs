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
            Directory.CreateDirectory(Path.Combine(_testDirectory, "subdir"));
            File.WriteAllText(Path.Combine(_testDirectory, "subdir", "file3.txt"), "Test File 3");

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
