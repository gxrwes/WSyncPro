using System;
using System.IO;
using System.Threading.Tasks;
using WSyncPro.Core.Models;
using WSyncPro.Core.State;
using Xunit;

namespace WSyncPro.Tests
{
    public class StateManagerTests : IDisposable
    {
        private readonly StateManager _stateManager;
        private readonly string _testLogFilePath;

        public StateManagerTests()
        {
            _stateManager = StateManager.Instance;
            _testLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "WSyncPro_Log.txt");

            // Clear log file if it exists to avoid interference between tests
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }
        }

        [Fact]
        public void Log_ShouldStoreMessagesCorrectly()
        {
            // Arrange
            string logMessage = "Test log message";

            // Act
            _stateManager.Log(logMessage);

            // Assert
            // Simulate waiting for log to be saved to avoid file access issues
            Task.Delay(500).Wait();
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(logMessage, logContent);
        }

        [Fact]
        public void UpdateProgress_ShouldUpdateProgressModelCorrectly()
        {
            // Arrange
            string serviceName = "TestService";
            string status = "In Progress";
            int percentageComplete = 50;

            // Act
            _stateManager.UpdateProgress(serviceName, status, percentageComplete);
            ProgressModel progress = _stateManager.GetLatestProgress(serviceName);

            // Assert
            Assert.NotNull(progress);
            Assert.Equal(serviceName, progress.ServiceName);
            Assert.Equal(status, progress.Status);
            Assert.Equal(percentageComplete, progress.PercentageComplete);
        }

        [Fact]
        public void SaveLog_ShouldRetryOnIOException()
        {
            // Arrange
            string logMessage = "Test log message with retry";

            // Simulate another process locking the log file
            using (var fileStream = File.Open(_testLogFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // Act
                Task logTask = Task.Run(() => _stateManager.Log(logMessage));

                // Assert
                // Verify that the log was eventually written after the file lock was released
                fileStream.Close(); // Release the lock

                // Wait for the logging task to complete with a reasonable timeout
                bool logWritten = logTask.Wait(TimeSpan.FromSeconds(2));

                // Now check the log content
                string logContent = File.ReadAllText(_testLogFilePath);
                Assert.Contains(logMessage, logContent);
            }
        }



        [Fact]
        public void SaveState_ShouldWriteToFileCorrectly()
        {
            // Arrange
            string testStateFilePath = "TestAppState.json";
            _stateManager.StateFilePath = testStateFilePath;

            // Act
            _stateManager.SaveState();

            // Assert
            Assert.True(File.Exists(testStateFilePath), "State file was not created.");
            File.Delete(testStateFilePath); // Clean up after the test
        }

        [Fact]
        public void LoadState_ShouldReadFromFileCorrectly()
        {
            // Arrange
            string testStateFilePath = "TestAppState.json";
            _stateManager.StateFilePath = testStateFilePath;

            var initialState = new AppState
            {
                Jobs = new System.Collections.Generic.List<Job>
                {
                    new Job { Id = "1", Name = "TestJob" }
                },
                SyncRuns = new System.Collections.Generic.List<SyncRun>()
            };

            File.WriteAllText(testStateFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(initialState));

            // Act
            _stateManager.LoadState();

            // Assert
            Assert.NotNull(_stateManager.Jobs);
            Assert.Single(_stateManager.Jobs);
            Assert.Equal("TestJob", _stateManager.Jobs[0].Name);

            File.Delete(testStateFilePath); // Clean up after the test
        }

        public void Dispose()
        {
            // Clean up resources after each test
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }
        }
    }
}
