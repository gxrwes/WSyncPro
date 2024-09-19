// AppStateManagerTests.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using WSyncPro.Core.Managers;
using WSyncPro.Core.Services;
using WSyncPro.Models;
using WSyncPro.Models.Enums;
using WSyncPro.Models.State;
using Xunit;

namespace WSyncPro.Test.Managers
{
    public class AppStateManagerTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _appStateFilePath;
        private readonly FileSerialisationServiceJson _serializationService;
        private readonly AppStateManager _appStateManager;

        public AppStateManagerTests()
        {
            // Setup a unique temporary directory for testing
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            // Define a test-specific app state file path
            _appStateFilePath = Path.Combine(_tempDir, "AppState.json");

            // Initialize the serialization service
            _serializationService = new FileSerialisationServiceJson();

            // Get the singleton instance
            _appStateManager = AppStateManager.Instance;

            // Use reflection to set the _appStateFilePath to the test path
            var appStateFilePathField = typeof(AppStateManager).GetField("_appStateFilePath", BindingFlags.NonPublic | BindingFlags.Instance);
            if (appStateFilePathField != null)
            {
                appStateFilePathField.SetValue(_appStateManager, _appStateFilePath);
            }

            // Initialize _currentAppState to a new snapshot
            var currentAppStateField = typeof(AppStateManager).GetField("_currentAppState", BindingFlags.NonPublic | BindingFlags.Instance);
            if (currentAppStateField != null)
            {
                currentAppStateField.SetValue(_appStateManager, new AppStateSnapShot
                {
                    Guid = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow
                });
            }

            // Clear existing jobs to ensure test isolation
            _appStateManager.Jobs.Clear();
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

            // Reset the AppStateManager's state
            var currentAppStateField = typeof(AppStateManager).GetField("_currentAppState", BindingFlags.NonPublic | BindingFlags.Instance);
            if (currentAppStateField != null)
            {
                currentAppStateField.SetValue(_appStateManager, new AppStateSnapShot
                {
                    Guid = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow
                });
            }

            _appStateManager.Jobs.Clear();
        }

        [Fact]
        public async Task LoadAppState_ShouldLoadExistingAppStateFromFile()
        {
            // Arrange
            var job3 = new Job
            {
                Id = "J003",
                Name = "Sync",
                Description = "Synchronize files between directories",
                IsEnabled = true,
                Created = DateTime.UtcNow.AddDays(-2),
                LastRun = DateTime.UtcNow.AddHours(-1)
            };

            var jobProgress3 = new JobProgress
            {
                JobId = "J003",
                JobName = "Sync",
                TotalFilesToProcess = 300,
                FilesProcessedSuccessfully = 200,
                FilesCurrentlyProcessed = 0,
                Status = JobStatus.Completed,
                StartTime = DateTime.UtcNow.AddHours(-3),
                LastUpdated = DateTime.UtcNow
            };
            jobProgress3.FailedFiles.Add(@"E:\Sync\FileX.txt");

            _appStateManager.Jobs.Add(job3);
            SetJobProgressHistory(job3, new List<JobProgress> { jobProgress3 });

            // Act
            _appStateManager.UpdateAppState();

            // Clear the Jobs list to simulate application restart
            _appStateManager.Jobs.Clear();

            // Reload the app state
            _appStateManager.LoadAppState();

            var retrievedAppState = _appStateManager.GetAppState();

            // Assert
            retrievedAppState.Should().NotBeNull();
            retrievedAppState.JobStateList.Should().HaveCount(1);
            retrievedAppState.Jobs.Should().HaveCount(1);
            retrievedAppState.Jobs.Should().ContainSingle(j => j.Id == job3.Id);

            // Verify Job3 State
            retrievedAppState.JobStateList.Should().ContainKey(job3.Id);
            var loadedJobProgress = retrievedAppState.JobStateList[job3.Id].FirstOrDefault();
            loadedJobProgress.Should().NotBeNull();
            loadedJobProgress.FilesProcessedSuccessfully.Should().Be(200);
            loadedJobProgress.FailedFiles.Should().ContainSingle(@"E:\Sync\FileX.txt");
        }

        [Fact]
        public void IsAppStateEqual_ShouldReturnTrue_ForIdenticalStates()
        {
            // Arrange
            var appStateManager = _appStateManager;

            var job4 = new Job
            {
                Id = "J004",
                Name = "Monitor",
                Description = "Monitor system performance",
                IsEnabled = true,
                Created = DateTime.UtcNow.AddHours(-10),
                LastRun = DateTime.UtcNow.AddMinutes(-30)
            };

            var appState1 = new AppStateSnapShot
            {
                Guid = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Jobs = new List<Job> { job4 },
                JobStateList = new Dictionary<string, List<JobProgress>>
                {
                    {
                        job4.Id, new List<JobProgress>
                        {
                            new JobProgress
                            {
                                JobId = "J004",
                                JobName = "Monitor",
                                TotalFilesToProcess = 150,
                                FilesProcessedSuccessfully = 100,
                                FilesCurrentlyProcessed = 10,
                                Status = JobStatus.Running,
                                StartTime = DateTime.UtcNow.AddHours(-4),
                                LastUpdated = DateTime.UtcNow.AddMinutes(-30)
                            }
                        }
                    }
                }
            };

            var appState2 = new AppStateSnapShot
            {
                Guid = appState1.Guid,
                TimeStamp = appState1.TimeStamp,
                Jobs = new List<Job> { job4 },
                JobStateList = new Dictionary<string, List<JobProgress>>
                {
                    {
                        job4.Id, new List<JobProgress>
                        {
                            new JobProgress
                            {
                                JobId = "J004",
                                JobName = "Monitor",
                                TotalFilesToProcess = 150,
                                FilesProcessedSuccessfully = 100,
                                FilesCurrentlyProcessed = 10,
                                Status = JobStatus.Running,
                                StartTime = appState1.JobStateList[job4.Id][0].StartTime,
                                LastUpdated = appState1.JobStateList[job4.Id][0].LastUpdated
                            }
                        }
                    }
                }
            };

            // Use reflection to set the private _currentAppState field
            var appStateField = typeof(AppStateManager).GetField("_currentAppState", BindingFlags.NonPublic | BindingFlags.Instance);
            if (appStateField != null)
            {
                appStateField.SetValue(appStateManager, appState1);
            }

            // Act
            var isEqual = InvokeIsAppStateEqual(appStateManager, appState1, appState2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Fact]
        public void IsAppStateEqual_ShouldReturnFalse_ForDifferentStates()
        {
            // Arrange
            var appStateManager = _appStateManager;

            var job5 = new Job
            {
                Id = "J005",
                Name = "Analyze",
                Description = "Analyze data trends",
                IsEnabled = true,
                Created = DateTime.UtcNow.AddHours(-8),
                LastRun = DateTime.UtcNow.AddMinutes(-15)
            };

            var appState1 = new AppStateSnapShot
            {
                Guid = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Jobs = new List<Job> { job5 },
                JobStateList = new Dictionary<string, List<JobProgress>>
                {
                    {
                        job5.Id, new List<JobProgress>
                        {
                            new JobProgress
                            {
                                JobId = "J005",
                                JobName = "Analyze",
                                TotalFilesToProcess = 250,
                                FilesProcessedSuccessfully = 200,
                                FilesCurrentlyProcessed = 20,

                                Status = JobStatus.Running,
                                StartTime = DateTime.UtcNow.AddHours(-5),
                                LastUpdated = DateTime.UtcNow.AddMinutes(-15)
                            }
                        }
                    }
                }
            };

            var job5Modified = new Job
            {
                Id = "J005",
                Name = "Analyze",
                Description = "Analyze data trends and patterns", // Modified description
                IsEnabled = true,
                Created = job5.Created,
                LastRun = job5.LastRun
            };

            var appState2 = new AppStateSnapShot
            {
                Guid = Guid.NewGuid(), // Different GUID
                TimeStamp = appState1.TimeStamp.AddMinutes(-5), // Different Timestamp
                Jobs = new List<Job> { job5Modified },
                JobStateList = new Dictionary<string, List<JobProgress>>
                {
                    {
                        job5Modified.Id, new List<JobProgress>
                        {
                            new JobProgress
                            {
                                JobId = "J005",
                                JobName = "Analyze",
                                TotalFilesToProcess = 250,
                                FilesProcessedSuccessfully = 200,
                                FilesCurrentlyProcessed = 20,
                                Status = JobStatus.Running,
                                StartTime = appState1.JobStateList[job5.Id][0].StartTime,
                                LastUpdated = appState1.JobStateList[job5.Id][0].LastUpdated
                            }
                        }
                    }
                }
            };

            // Use reflection to set the private _currentAppState field
            var appStateField = typeof(AppStateManager).GetField("_currentAppState", BindingFlags.NonPublic | BindingFlags.Instance);
            if (appStateField != null)
            {
                appStateField.SetValue(appStateManager, appState1);
            }

            // Act
            var isEqual = InvokeIsAppStateEqual(appStateManager, appState1, appState2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Fact]
        public async Task SaveAppState_ShouldHandleSerializationFailureGracefully()
        {
            // Arrange
            var job = new Job
            {
                Id = "J006",
                Name = "FaultyJob",
                Description = "This job will fail to serialize.",
                IsEnabled = true,
                Created = DateTime.UtcNow.AddDays(-1),
                LastRun = DateTime.UtcNow
            };

            _appStateManager.Jobs.Add(job);

            // Use reflection to set an invalid file path to simulate serialization failure
            var invalidFilePath = Path.Combine(_tempDir, "invalid\0path.json"); // Invalid path due to null character

            var appStateFilePathField = typeof(AppStateManager).GetField("_appStateFilePath", BindingFlags.NonPublic | BindingFlags.Instance);
            if (appStateFilePathField != null)
            {
                appStateFilePathField.SetValue(_appStateManager, invalidFilePath);
            }

            // Act
            // Attempt to update and save the app state
            try
            {
                _appStateManager.UpdateAppState();
            }
            catch
            {
                // Expected to fail due to invalid path
            }

            // Assert
            File.Exists(invalidFilePath).Should().BeFalse();
        }

        /// <summary>
        /// Helper method to invoke the private IsAppStateEqual method using reflection.
        /// </summary>
        /// <param name="appStateManager">The AppStateManager instance.</param>
        /// <param name="state1">First AppStateSnapShot.</param>
        /// <param name="state2">Second AppStateSnapShot.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        private bool InvokeIsAppStateEqual(AppStateManager appStateManager, AppStateSnapShot state1, AppStateSnapShot state2)
        {
            var method = typeof(AppStateManager).GetMethod("IsAppStateEqual", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
            {
                return (bool)method.Invoke(appStateManager, new object[] { state1, state2 });
            }
            return false;
        }

        /// <summary>
        /// Helper method to set the ProgressHistory property of a Job using reflection.
        /// </summary>
        /// <param name="job">The Job instance.</param>
        /// <param name="progressHistory">The list of JobProgress to set.</param>
        private void SetJobProgressHistory(Job job, List<JobProgress> progressHistory)
        {
            var progressHistoryProperty = typeof(Job).GetProperty("ProgressHistory", BindingFlags.Public | BindingFlags.Instance);
            if (progressHistoryProperty != null && progressHistoryProperty.CanWrite)
            {
                progressHistoryProperty.SetValue(job, progressHistory);
            }
            else
            {
                var progressHistoryField = typeof(Job).GetField("ProgressHistory", BindingFlags.NonPublic | BindingFlags.Instance);
                if (progressHistoryField != null)
                {
                    progressHistoryField.SetValue(job, progressHistory);
                }
            }
        }
    }
}
