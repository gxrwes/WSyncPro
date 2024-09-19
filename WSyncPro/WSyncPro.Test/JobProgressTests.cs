using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using WSyncPro.Models.State;
using WSyncPro.Core.Services;
using Xunit;
using WSyncPro.Models;
using WSyncPro.Models.Enums;

namespace WSyncPro.Test
{
    public class JobProgressTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _testFilePath;
        private readonly FileSerialisationServiceJson _serializationService;

        public JobProgressTests()
        {
            // Create a unique temporary directory for testing
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            // Define the test JSON file path
            _testFilePath = Path.Combine(_tempDir, "jobprogress.json");

            // Initialize the serialization service
            _serializationService = new FileSerialisationServiceJson();
        }

        public void Dispose()
        {
            // Clean up the temporary directory after tests
            if (Directory.Exists(_tempDir))
            {
                try
                {
                    Directory.Delete(_tempDir, true);
                }
                catch
                {
                    // Ignore any exceptions during cleanup
                }
            }
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldSaveJobProgressToJsonFile()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                JobId = "JOB123",
                JobName = "Data Backup",
                TotalFilesToProcess = 200,
                FilesProcessedSuccessfully = 150,
                FilesCurrentlyProcessed = 10,
                Status = JobStatus.Running,
                StartTime = DateTime.UtcNow.AddHours(-1),
                LastUpdated = DateTime.UtcNow
            };

            // Add failed files
            jobProgress.FailedFiles.Add(@"C:\Source\File1.txt");
            jobProgress.FailedFiles.Add(@"C:\Source\File2.docx");

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(_testFilePath, jobProgress);

            // Assert
            result.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            // Deserialize back to verify
            JobProgress deserializedJobProgress = await _serializationService.GetFileAsClassAsync<JobProgress>(_testFilePath);
            deserializedJobProgress.Should().NotBeNull();
            deserializedJobProgress.JobId.Should().Be("JOB123");
            deserializedJobProgress.JobName.Should().Be("Data Backup");
            deserializedJobProgress.TotalFilesToProcess.Should().Be(200);
            deserializedJobProgress.FilesProcessedSuccessfully.Should().Be(150);
            deserializedJobProgress.FilesCurrentlyProcessed.Should().Be(10);
            deserializedJobProgress.Status.Should().Be(JobStatus.Running);
            deserializedJobProgress.StartTime.Should().BeCloseTo(jobProgress.StartTime, TimeSpan.FromSeconds(1));
            deserializedJobProgress.LastUpdated.Should().BeCloseTo(jobProgress.LastUpdated, TimeSpan.FromSeconds(1));
            deserializedJobProgress.FailedFiles.Should().HaveCount(2);
            deserializedJobProgress.FailedFiles.Should().Contain(new[] { @"C:\Source\File1.txt", @"C:\Source\File2.docx" });
            deserializedJobProgress.FilesFailed.Should().Be(2);
        }

        [Fact]
        public async Task GetFileAsClassAsync_ShouldDeserializeJobProgressFromJsonFile()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                JobId = "JOB456",
                JobName = "File Synchronization",
                TotalFilesToProcess = 500,
                FilesProcessedSuccessfully = 400,
                FilesCurrentlyProcessed = 20,
                Status = JobStatus.Completed,
                StartTime = DateTime.UtcNow.AddHours(-2),
                LastUpdated = DateTime.UtcNow
            };

            // Add failed files
            jobProgress.FailedFiles.Add(@"D:\Source\FileA.txt");
            jobProgress.FailedFiles.Add(@"D:\Source\FileB.docx");
            jobProgress.FailedFiles.Add(@"D:\Source\FileC.tmp");
            jobProgress.FailedFiles.Add(@"D:\Source\FileD.log");

            // Act
            bool saveResult = await _serializationService.SaveClassToFileAsync(_testFilePath, jobProgress);
            saveResult.Should().BeTrue();

            JobProgress deserializedJobProgress = await _serializationService.GetFileAsClassAsync<JobProgress>(_testFilePath);

            // Assert
            deserializedJobProgress.Should().NotBeNull();
            deserializedJobProgress.JobId.Should().Be("JOB456");
            deserializedJobProgress.JobName.Should().Be("File Synchronization");
            deserializedJobProgress.TotalFilesToProcess.Should().Be(500);
            deserializedJobProgress.FilesProcessedSuccessfully.Should().Be(400);
            deserializedJobProgress.FilesCurrentlyProcessed.Should().Be(20);
            deserializedJobProgress.Status.Should().Be(JobStatus.Completed);
            deserializedJobProgress.StartTime.Should().BeCloseTo(jobProgress.StartTime, TimeSpan.FromSeconds(1));
            deserializedJobProgress.LastUpdated.Should().BeCloseTo(jobProgress.LastUpdated, TimeSpan.FromSeconds(1));
            deserializedJobProgress.FailedFiles.Should().HaveCount(4);
            deserializedJobProgress.FailedFiles.Should().Contain(new[]
            {
                @"D:\Source\FileA.txt",
                @"D:\Source\FileB.docx",
                @"D:\Source\FileC.tmp",
                @"D:\Source\FileD.log"
            });
            deserializedJobProgress.FilesFailed.Should().Be(4);
        }

        [Fact]
        public async Task GetCompletionPercentage_ShouldReturnCorrectPercentage()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                TotalFilesToProcess = 100,
                FilesProcessedSuccessfully = 60,
                FilesCurrentlyProcessed = 5
            };

            // Add failed files
            jobProgress.FailedFiles.Add("FailedFile1.txt");
            jobProgress.FailedFiles.Add("FailedFile2.docx");

            // Act
            float completionPercentage = jobProgress.GetCompletionPercentage();

            // Assert
            // Total processed = 60 (successful) + 2 (failed files count)
            // Completion Percentage = (62 / 100) * 100 = 62%
            completionPercentage.Should().BeApproximately(62f, 0.01f);
        }

        [Fact]
        public async Task GetCompletionPercentage_ShouldReturnZero_WhenTotalFilesToProcessIsZero()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                TotalFilesToProcess = 0,
                FilesProcessedSuccessfully = 0,
                FilesCurrentlyProcessed = 0
            };

            // Act
            float completionPercentage = jobProgress.GetCompletionPercentage();

            // Assert
            completionPercentage.Should().Be(0f);
        }

        [Fact]
        public async Task GetCompletionPercentage_ShouldNotExceed100Percent()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                TotalFilesToProcess = 100,
                FilesProcessedSuccessfully = 90,
                FilesCurrentlyProcessed = 0
            };

            // Add failed files
            for (int i = 0; i < 20; i++)
            {
                jobProgress.FailedFiles.Add($"FailedFile{i}.txt");
            }

            // Act
            float completionPercentage = jobProgress.GetCompletionPercentage();

            // Assert
            completionPercentage.Should().Be(100f);
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldHandleNullFailedFilesList()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                JobId = "JOB789",
                JobName = "Data Migration",
                TotalFilesToProcess = 300,
                FilesProcessedSuccessfully = 250,
                FilesCurrentlyProcessed = 30,
                Status = JobStatus.Paused,
                StartTime = DateTime.UtcNow.AddMinutes(-30),
                LastUpdated = DateTime.UtcNow
            };

            // Simulate null by clearing the collection
            jobProgress.FailedFiles.Clear();

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(_testFilePath, jobProgress);

            // Assert
            result.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            JobProgress deserializedJobProgress = await _serializationService.GetFileAsClassAsync<JobProgress>(_testFilePath);
            deserializedJobProgress.Should().NotBeNull();
            deserializedJobProgress.FailedFiles.Should().BeEmpty();
            deserializedJobProgress.FilesFailed.Should().Be(0);
        }

        [Fact]
        public void GetCompletionPercentage_ShouldHandleNegativeValuesGracefully()
        {
            // Arrange
            var jobProgress = new JobProgress
            {
                TotalFilesToProcess = -10,
                FilesProcessedSuccessfully = -5,
                FilesCurrentlyProcessed = -2
            };

            // Add failed files (negative count isn't directly representable, so no additions)
            // Alternatively, if you need to simulate negative counts, you might have to adjust the class to allow it
            // For the sake of this test, we'll consider it as zero or handle gracefully

            // Act
            float completionPercentage = jobProgress.GetCompletionPercentage();

            // Assert
            // Since TotalFilesToProcess is negative, the method should return 0%
            completionPercentage.Should().Be(0f);
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldReturnFalse_WhenSerializationFails()
        {
            // Arrange
            var job = new Job
            {
                Id = "J002",
                Name = "Cleanup",
                Created = DateTime.UtcNow.AddDays(-3),
                LastRun = DateTime.UtcNow,
                Description = "Clean up temporary files",
                IsEnabled = true
            };

            // Create a file that will act as a directory
            string invalidDirPath = Path.Combine(_tempDir, "testfile.json");
            File.WriteAllText(invalidDirPath, "This is a file, not a directory.");

            // Now, attempt to serialize to "testfile.json\file.json", which should fail
            string invalidFilePath = Path.Combine(invalidDirPath, "file.json");

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(invalidFilePath, job);

            // Assert
            result.Should().BeFalse();
            File.Exists(invalidFilePath).Should().BeFalse();
        }
    }
}
