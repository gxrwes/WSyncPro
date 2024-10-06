﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using WSyncPro.Core.Managers;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.State;
using WSyncPro.Util.Services;
using Xunit;

namespace WSyncPro.Test.Unit.Core.Services
{
    public class SyncServiceUnitTest
    {
        private readonly Mock<IDirectoryScannerService> _mockScannerService;
        private readonly Mock<IFileCopyMoveService> _mockFileCopyMoveService;
        private readonly StateManager _stateManager;
        private readonly SyncService _syncService;

        public SyncServiceUnitTest()
        {
            _mockScannerService = new Mock<IDirectoryScannerService>();
            _mockFileCopyMoveService = new Mock<IFileCopyMoveService>();
            _stateManager = StateManager.Instance;

            _syncService = new SyncService(_mockScannerService.Object, _mockFileCopyMoveService.Object, _stateManager);

            // Reset the state manager's JobStates dictionary before each test
            _stateManager.JobStates.Clear();
        }

        [Fact]
        public async Task RunSyncAsync_ShouldScanDirectoryAndCopyFiles()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                SrcDirectory = "/src",
                DstDirectory = "/dst",
                FilterInclude = new List<string> { "*.txt" }
            };

            var mockFiles = new List<WObject>
            {
                new WFile { Name = "file1.txt", FullPath = "/src/file1.txt" },
                new WFile { Name = "file2.txt", FullPath = "/src/file2.txt" }
            };

            _mockScannerService.Setup(x => x.ScanAsync(syncJob))
                .ReturnsAsync(mockFiles);

            _mockFileCopyMoveService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var report = await _syncService.RunSyncAsync(syncJob);

            // Assert
            _mockScannerService.Verify(x => x.ScanAsync(syncJob), Times.Once);
            _mockFileCopyMoveService.Verify(x => x.CopyFileAsync("/src/file1.txt", Path.Combine(syncJob.DstDirectory, "file1.txt")), Times.Once);
            _mockFileCopyMoveService.Verify(x => x.CopyFileAsync("/src/file2.txt", Path.Combine(syncJob.DstDirectory, "file2.txt")), Times.Once);
            Assert.Equal(2, report.TouchedObjects.Count);
            Assert.Equal(0, report.IgnoredItems);
            Assert.Equal(2, _stateManager.JobStates[syncJob].ItemsProcessed);
        }

        [Fact]
        public async Task RunSyncAsync_ShouldIgnoreOlderFiles()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                SrcDirectory = "/src",
                DstDirectory = "/dst"
            };

            var mockFiles = new List<WObject>
            {
                new WFile { Name = "oldFile.txt", FullPath = "/src/oldFile.txt" }
            };

            _mockScannerService.Setup(x => x.ScanAsync(syncJob))
                .ReturnsAsync(mockFiles);

            // Simulate that the destination file already exists and is newer than the source file
            var destinationFilePath = Path.Combine(syncJob.DstDirectory, "oldFile.txt");
            File.Create(destinationFilePath).Close();
            File.SetLastWriteTimeUtc(destinationFilePath, DateTime.UtcNow.AddHours(1)); // Make destination file "newer"

            // Act
            var report = await _syncService.RunSyncAsync(syncJob);

            // Assert
            _mockScannerService.Verify(x => x.ScanAsync(syncJob), Times.Once);
            _mockFileCopyMoveService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.Empty(report.TouchedObjects); // No files should be copied
            Assert.Equal(1, report.IgnoredItems);

            // Cleanup
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }
        }

        [Fact]
        public async Task RunSyncAsync_ShouldUpdateJobStateDuringSync()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                SrcDirectory = "/src",
                DstDirectory = "/dst"
            };

            var mockFiles = new List<WObject>
            {
                new WFile { Name = "file1.txt", FullPath = "/src/file1.txt" }
            };

            _mockScannerService.Setup(x => x.ScanAsync(syncJob))
                .ReturnsAsync(mockFiles);

            _mockFileCopyMoveService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _syncService.RunSyncAsync(syncJob);

            // Assert
            Assert.Equal(1, _stateManager.JobStates[syncJob].ItemsProcessed);
            Assert.Equal(1, _stateManager.JobStates[syncJob].TotalItemToProcess);
        }

        [Fact]
        public async Task RunSyncAsync_ShouldGenerateCorrectReport()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                SrcDirectory = "/src",
                DstDirectory = "/dst",
                FilterInclude = new List<string> { "*.txt" },
                FilterExclude = new List<string> { "*.log" }
            };

            var mockFiles = new List<WObject>
            {
                new WFile { Name = "file1.txt", FullPath = "/src/file1.txt" },
                new WFile { Name = "file2.log", FullPath = "/src/file2.log" }
            };

            _mockScannerService.Setup(x => x.ScanAsync(syncJob))
                .ReturnsAsync(mockFiles);

            _mockFileCopyMoveService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var report = await _syncService.RunSyncAsync(syncJob);

            // Assert
            _mockScannerService.Verify(x => x.ScanAsync(syncJob), Times.Once);
            _mockFileCopyMoveService.Verify(x => x.CopyFileAsync("/src/file1.txt", Path.Combine(syncJob.DstDirectory, "file1.txt")), Times.Once);
            _mockFileCopyMoveService.Verify(x => x.CopyFileAsync("/src/file2.log", Path.Combine(syncJob.DstDirectory, "file2.log")), Times.Never); // Shouldn't copy .log file due to FilterExclude
            Assert.Single(report.TouchedObjects); // Only 1 file should be copied
            Assert.Equal(1, report.IgnoredItems); // 1 file should be ignored
        }
    }
}