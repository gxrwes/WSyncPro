using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services;
using WSyncPro.Core.Services.ScanService;
using Xunit;

namespace WSyncPro.Tests
{
    public class SyncServiceTests
    {
        private readonly SyncService _syncService;
        private readonly DirectoryScanner _realDirectoryScanner; // Use real instance
        private readonly CopyService _realCopyService; // Use real instance

        public SyncServiceTests()
        {
            // Initialize a real DirectoryScanner and CopyService
            _realDirectoryScanner = new DirectoryScanner();
            _realCopyService = new CopyService();

            // Pass the real DirectoryScanner and real CopyService to the SyncService
            _syncService = new SyncService(_realDirectoryScanner, _realCopyService);
        }

        [Fact]
        public void ExecuteJobs_SkipsDisabledJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { IsEnabled = false, SourceDirectory = "sourcePath", TargetDirectory = "targetPath" }
            };

            // Act
            _syncService.ExecuteJobs(jobs);

            // Assert - Ensure the target directory is not created, implying no copy was attempted
            Assert.False(Directory.Exists("targetPath"));
        }

        [Fact]
        public void ExecuteJobs_ThrowsExceptionWhenSourceDirectoryIsInvalid()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { IsEnabled = true, SourceDirectory = "invalidPath", TargetDirectory = "targetPath" }
            };

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => _syncService.ExecuteJobs(jobs));
        }

        [Fact]
        public void ExecuteJobs_ScansDirectoryWhenNoProvidedDirectory()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { IsEnabled = true, SourceDirectory = "sourcePath", TargetDirectory = "targetPath" }
            };

            Directory.CreateDirectory("sourcePath");
            File.WriteAllText(Path.Combine("sourcePath", "test.txt"), "test content");

            // Act
            _syncService.ExecuteJobs(jobs);

            // Assert - Ensure the file is copied
            Assert.True(File.Exists(Path.Combine("targetPath", "test.txt")));

            // Clean up
            Directory.Delete("sourcePath", true);
            Directory.Delete("targetPath", true);
        }

        [Fact]
        public void ExecuteJobs_UsesProvidedDirectory()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { IsEnabled = true, SourceDirectory = "sourcePath", TargetDirectory = "targetPath" }
            };

            var providedDirectory = new WDirectory
            {
                Path = "sourcePath",
                Files = new List<WFile>
                {
                    new WFile { Name = "test.txt", Path = "sourcePath/test.txt" }
                }
            };

            Directory.CreateDirectory("sourcePath");
            File.WriteAllText(Path.Combine("sourcePath", "test.txt"), "test content");

            // Act
            _syncService.ExecuteJobs(jobs, providedDirectory);

            // Assert - Ensure the file is moved
            Assert.True(File.Exists(Path.Combine("targetPath", "test.txt")));

            // Clean up
            Directory.Delete("sourcePath", true);
            Directory.Delete("targetPath", true);
        }
    }
}
