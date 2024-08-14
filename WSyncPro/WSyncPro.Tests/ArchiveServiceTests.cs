using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services;
using WSyncPro.Core.Services.ArchiveServices;
using WSyncPro.Core.Services.ScanService;
using Xunit;

namespace WSyncPro.Tests
{
    public class ArchiveServiceTests : IDisposable
    {
        private readonly ArchiveService _archiveService;
        private readonly DirectoryScanner _directoryScanner;
        private readonly ArchiveZiper _archiveZiper;
        private readonly string _testDirectory;
        private readonly string _targetDirectory;

        public ArchiveServiceTests()
        {
            _directoryScanner = new DirectoryScanner();
            _archiveZiper = new ArchiveZiper();
            _archiveService = new ArchiveService(_directoryScanner, _archiveZiper);

            // Setup a temporary directory for testing
            _testDirectory = Path.Combine(Path.GetTempPath(), "ArchiveServiceTest");
            _targetDirectory = Path.Combine(_testDirectory, "Target");
            Directory.CreateDirectory(_testDirectory);

            // Create some test files
            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "This is a test file.");
        }

        [Fact]
        public void ExecuteArchive_ThrowsException_WhenJobIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _archiveService.ExecuteArchive(null));
        }

        [Fact]
        public void ExecuteArchive_UsesProvidedDirectory()
        {
            // Arrange
            var job = new Job
            {
                ArchiveOptions = new ArchiveOptions
                {
                    GroupingPattern = "YEAR\\MONTH\\DAY"
                },
                TargetDirectory = _targetDirectory
            };

            var providedDirectory = new WDirectory
            {
                Name = "TestDir",
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testDirectory, "file1.txt") }
                }
            };

            // Act
            _archiveService.ExecuteArchive(job, providedDirectory);

            // Construct the expected path considering the date structure
            var today = DateTime.Now;
            var expectedZipPath = Path.Combine(
                _targetDirectory,
                today.Year.ToString(),
                today.Month.ToString("D2"),
                today.Day.ToString("D2"),
                $"{providedDirectory.Name}.zip"
            );

            // Assert
            Assert.True(File.Exists(expectedZipPath), $"Expected zip file {expectedZipPath} does not exist.");
        }

        [Fact]
        public void ExecuteArchive_ScansDirectory_WhenNoDirectoryIsProvided()
        {
            // Arrange
            var job = new Job
            {
                SourceDirectory = _testDirectory,
                ArchiveOptions = new ArchiveOptions
                {
                    GroupingPattern = "YEAR\\MONTH\\DAY"
                },
                TargetDirectory = _targetDirectory
            };

            // Act
            _archiveService.ExecuteArchive(job);

            // Construct the expected path considering the date structure
            var today = DateTime.Now;
            var expectedZipPath = Path.Combine(
                _targetDirectory,
                today.Year.ToString(),
                today.Month.ToString("D2"),
                today.Day.ToString("D2"),
                $"{Path.GetFileName(_testDirectory)}.zip"
            );

            // Assert
            Assert.True(File.Exists(expectedZipPath), $"Expected zip file {expectedZipPath} does not exist.");
        }

        public void Dispose()
        {
            // Clean up the test directory after each test
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
