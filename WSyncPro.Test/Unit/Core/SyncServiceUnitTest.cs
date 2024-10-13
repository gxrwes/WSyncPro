using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Enum;
using WSyncPro.Models.Data;
using WSyncPro.Util.Files;
using WSyncPro.Util.Services;
using Xunit;

namespace WSyncPro.Test.Unit.Core
{
    public class SyncServiceUnitTest
    {
        private readonly Mock<IFileLoader> _fileLoaderMock;
        private readonly Mock<IDirectoryScannerService> _directoryScannerServiceMock;
        private readonly Mock<IFileCopyMoveService> _fileCopyMoveServiceMock;
        private readonly SyncService _syncService;

        public SyncServiceUnitTest()
        {
            _fileLoaderMock = new Mock<IFileLoader>();
            _directoryScannerServiceMock = new Mock<IDirectoryScannerService>();
            _fileCopyMoveServiceMock = new Mock<IFileCopyMoveService>();
            _syncService = new SyncService(_fileLoaderMock.Object, _directoryScannerServiceMock.Object, _fileCopyMoveServiceMock.Object);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task AddJob_ShouldAddNewJob()
        {
            // Arrange
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Job1", Status = Status.Pending };

            // Act
            await _syncService.AddJob(job);
            var jobs = await _syncService.GetAllJobs();

            // Assert
            Assert.Single(jobs);
            Assert.Equal("Job1", jobs[0].Name);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task AddJob_ShouldUpdateExistingJob()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var job = new SyncJob { Id = jobId, Name = "Job1", Status = Status.Pending };
            var updatedJob = new SyncJob { Id = jobId, Name = "UpdatedJob", Status = Status.Running };

            await _syncService.AddJob(job);

            // Act
            await _syncService.AddJob(updatedJob);
            var jobs = await _syncService.GetAllJobs();

            // Assert
            Assert.Single(jobs);
            Assert.Equal("UpdatedJob", jobs[0].Name);
            Assert.Equal(Status.Running, jobs[0].Status);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task LoadJoblistFromFile_ShouldLoadJobsFromFile()
        {
            // Arrange
            var joblistFilePath = "joblist.json";
            var jobList = new List<SyncJob>
            {
                new SyncJob { Id = Guid.NewGuid(), Name = "Job1", Status = Status.Running },
                new SyncJob { Id = Guid.NewGuid(), Name = "Job2", Status = Status.Pending }
            };

            _fileLoaderMock.Setup(fl => fl.LoadFileAndParseAsync<List<SyncJob>>(joblistFilePath))
                .ReturnsAsync(jobList);

            // Act
            await _syncService.LoadJoblistFromFile(joblistFilePath);
            var jobs = await _syncService.GetAllJobs();

            // Assert
            Assert.Equal(2, jobs.Count);
            Assert.Equal("Job1", jobs[0].Name);
            Assert.Equal("Job2", jobs[1].Name);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task SaveJoblistToFile_ShouldSaveJobsToFile()
        {
            // Arrange
            var joblistFilePath = "joblist.json";
            var jobList = new List<SyncJob>
            {
                new SyncJob { Id = Guid.NewGuid(), Name = "Job1", Status = Status.Running },
                new SyncJob { Id = Guid.NewGuid(), Name = "Job2", Status = Status.Pending }
            };

            foreach (var job in jobList)
            {
                await _syncService.AddJob(job);
            }

            // Act
            await _syncService.SaveJoblistToFile(joblistFilePath);

            // Assert
            _fileLoaderMock.Verify(fl => fl.SaveToFileAsObjectAsync(joblistFilePath, It.Is<List<SyncJob>>(jobs => jobs.Count == 2)), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task RunAllEnabledJobs_ShouldProcessEnabledJobs()
        {
            // Arrange
            var job1 = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Job1",
                SrcDirectory = "/source1",
                DstDirectory = "/dest1",
                Status = Status.Running
            };

            var job2 = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Job2",
                SrcDirectory = "/source2",
                DstDirectory = "/dest2",
                Status = Status.Disabled
            };

            var filesToSync = new List<WObject>
    {
        new WFile { Name = "file1.txt", FullPath = "/source1/file1.txt" }
    };

            await _syncService.AddJob(job1);
            await _syncService.AddJob(job2);

            _directoryScannerServiceMock.Setup(ds => ds.ScanAsync(job1)).ReturnsAsync(filesToSync);
            _fileCopyMoveServiceMock.Setup(fs => fs.CopyFileAsync(It.Is<string>(s => s.EndsWith("file1.txt")),
                                                                 It.Is<string>(s => s.EndsWith("file1.txt"))))
                                     .Returns(Task.CompletedTask);

            // Act
            var result = await _syncService.RunAllEnabledJobs();

            // Assert
            Assert.Equal(job1.Id, result.Item1);
            Assert.Equal(1, result.Item2);
            Assert.Equal("Jobs Completed", result.Item3);

            _directoryScannerServiceMock.Verify(ds => ds.ScanAsync(job1), Times.Once);
            _fileCopyMoveServiceMock.Verify(fs => fs.CopyFileAsync(It.Is<string>(s => s.EndsWith("file1.txt")),
                                                                  It.Is<string>(s => s.EndsWith("file1.txt"))), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit Test")]

        public async Task RunAllEnabledJobs_ShouldNotProcessDisabledJobs()
        {
            // Arrange
            var job = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "DisabledJob",
                SrcDirectory = "/source",
                DstDirectory = "/dest",
                Status = Status.Disabled
            };

            await _syncService.AddJob(job);

            // Act
            var result = await _syncService.RunAllEnabledJobs();

            // Assert
            Assert.Equal(Guid.Empty, result.Item1);
            Assert.Equal(0, result.Item2);
            Assert.Equal("Jobs Completed", result.Item3);

            _directoryScannerServiceMock.Verify(ds => ds.ScanAsync(It.IsAny<SyncJob>()), Times.Never);
            _fileCopyMoveServiceMock.Verify(fs => fs.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
