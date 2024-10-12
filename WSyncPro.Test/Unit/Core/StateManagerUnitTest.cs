using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WSyncPro.Core.Managers;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;
using Xunit;

namespace WSyncPro.Test.Unit.Core.Managers
{
    public class StateManagerUnitTest : IDisposable
    {
        private readonly string tempDirectory;
        private readonly string jobListFilePath;

        public StateManagerUnitTest()
        {
            // Create a temporary directory for testing
            tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            // Define the job list file path within the temporary directory
            jobListFilePath = Path.Combine(tempDirectory, "joblist.json");
        }

        public void Dispose()
        {
            // Clean up the temporary directory after tests run
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public void StateManager_ShouldInitializeJobStates()
        {
            // Arrange
            var stateManager = new StateManager(jobListFilePath);

            // Act & Assert
            Assert.NotNull(stateManager.JobStates);
            Assert.Empty(stateManager.JobStates);
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task StateManager_ShouldAddSyncJobState()
        {
            // Arrange
            var stateManager = new StateManager(jobListFilePath);
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            var jobState = new SyncJobState { TotalItemToProcess = 10 };

            // Act
            await stateManager.AddJob(job);
            stateManager.JobStates[job] = jobState;

            // Assert
            Assert.True(stateManager.JobStates.ContainsKey(job));
            Assert.Equal(10, stateManager.JobStates[job].TotalItemToProcess);
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task StateManager_ShouldUpdateJobState()
        {
            // Arrange
            var stateManager = new StateManager(jobListFilePath);
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            var jobState = new SyncJobState { TotalItemToProcess = 10 };
            await stateManager.AddJob(job);
            stateManager.JobStates[job] = jobState;

            // Act
            stateManager.JobStates[job].ItemsProcessed = 5;

            // Assert
            Assert.Equal(5, stateManager.JobStates[job].ItemsProcessed);
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task StateManager_ShouldLoadJobsFromFile()
        {
            // Arrange
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            var jobs = new List<SyncJob> { job };

            // Serialize the job list to the job list file path
            var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }, WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(jobs, options);
            await File.WriteAllTextAsync(jobListFilePath, jsonContent);

            // Act
            var stateManager = new StateManager(jobListFilePath);
            await stateManager.TryLoadJobsFromFile();

            // Assert
            Assert.True(stateManager.JobStates.ContainsKey(job));
            Assert.Equal(job.Name, job.Name);
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task StateManager_ShouldSaveJobsToFile()
        {
            // Arrange
            var stateManager = new StateManager(jobListFilePath);
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            await stateManager.AddJob(job);

            // Act
            await stateManager.TrySaveJobsToFile();

            // Assert
            Assert.True(File.Exists(jobListFilePath));
            var fileContent = await File.ReadAllTextAsync(jobListFilePath);
            var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
            var jobsFromFile = JsonSerializer.Deserialize<List<SyncJob>>(fileContent, options);
            Assert.Single(jobsFromFile);
            Assert.Equal(job.Id, jobsFromFile[0].Id);
            Assert.Equal(job.Name, jobsFromFile[0].Name);
        }
    }
}
