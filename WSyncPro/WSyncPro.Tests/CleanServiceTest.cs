using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services;
using WSyncPro.Core.Services.CleanService;
using WSyncPro.Core.Services.ScanService;
using WSyncPro.Core.State;
using Xunit;

namespace WSyncPro.Tests
{
    public class CleanServiceTests : IDisposable
    {
        private readonly CleanService _cleanService;
        private readonly DirectoryScanner _directoryScanner;
        private readonly MoveService _moveService;
        private readonly string _testDirectory;
        private readonly string _trashDirectory;

        public CleanServiceTests()
        {
            _directoryScanner = new DirectoryScanner();
            _moveService = new MoveService();
            _cleanService = new CleanService(_directoryScanner, _moveService);

            // Setup a temporary directory for testing
            _testDirectory = Path.Combine(Path.GetTempPath(), "CleanServiceTest");
            _trashDirectory = Path.Combine(Path.GetTempPath(), "Trash");
            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_trashDirectory);

            // Set the TrashDirectory in StateManager
            StateManager.Instance.TrashDirectory = _trashDirectory;

            // Create some test files
            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "This is a test file.");
        }

        [Fact]
        public void ExecuteClean_ThrowsException_WhenTrashDirectoryIsNotSet()
        {
            // Arrange
            StateManager.Instance.TrashDirectory = null; // Unset the TrashDirectory
            var job = new Job { SourceDirectory = _testDirectory };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _cleanService.ExecuteClean(job));
        }

        [Fact]
        public void ExecuteClean_ThrowsException_WhenTrashDirectoryIsInvalid()
        {
            // Arrange
            StateManager.Instance.TrashDirectory = "InvalidPath:://"; // Set an invalid TrashDirectory path
            var job = new Job { SourceDirectory = _testDirectory };

            // Act & Assert
            Assert.Throws<System.IO.IOException>(() => _cleanService.ExecuteClean(job));
        }


        [Fact]
        public void ExecuteClean_UsesProvidedDirectory()
        {
            // Arrange
            var job = new Job { SourceDirectory = _testDirectory };
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
            _cleanService.ExecuteClean(job, providedDirectory);

            // Assert
            var expectedFilePath = Path.Combine(_trashDirectory, "file1.txt");
            Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} does not exist in trash.");
        }

        [Fact]
        public void ExecuteClean_ScansDirectory_WhenNoDirectoryIsProvided()
        {
            // Arrange
            var job = new Job { SourceDirectory = _testDirectory };

            // Act
            _cleanService.ExecuteClean(job);

            // Assert
            var expectedFilePath = Path.Combine(_trashDirectory, "file1.txt");
            Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} does not exist in trash.");
        }

        public void Dispose()
        {
            // Clean up the test and trash directories after each test
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
            if (Directory.Exists(_trashDirectory))
            {
                Directory.Delete(_trashDirectory, true);
            }
        }
    }
}
