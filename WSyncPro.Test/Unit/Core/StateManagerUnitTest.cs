using System;
using System.Collections.Generic;
using WSyncPro.Core.Managers;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;
using Xunit;

namespace WSyncPro.Test.Unit.Core.Managers
{
    public class StateManagerUnitTest
    {
        [Fact]
        [Trait("Category", "Unit Test")]

        public void StateManager_ShouldBeSingleton()
        {
            // Arrange
            var stateManager1 = StateManager.Instance;
            var stateManager2 = StateManager.Instance;

            // Act & Assert
            Assert.Same(stateManager1, stateManager2); // Both should be the same instance
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public void StateManager_ShouldInitializeJobStates()
        {
            // Arrange
            var stateManager = StateManager.Instance;

            // Act & Assert
            Assert.NotNull(stateManager.JobStates);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public void StateManager_ShouldAddSyncJobState()
        {
            // Arrange
            var stateManager = StateManager.Instance;
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            var jobState = new SyncJobState { TotalItemToProcess = 10 };

            // Act
            stateManager.JobStates[job] = jobState;

            // Assert
            Assert.True(stateManager.JobStates.ContainsKey(job));
            Assert.Equal(10, stateManager.JobStates[job].TotalItemToProcess);
        }

        [Fact]
        [Trait("Category", "Unit Test")]

        public void StateManager_ShouldUpdateJobState()
        {
            // Arrange
            var stateManager = StateManager.Instance;
            var job = new SyncJob { Id = Guid.NewGuid(), Name = "Test Job" };
            var jobState = new SyncJobState { TotalItemToProcess = 10 };
            stateManager.JobStates[job] = jobState;

            // Act
            stateManager.JobStates[job].ItemsProcessed = 5;

            // Assert
            Assert.Equal(5, stateManager.JobStates[job].ItemsProcessed);
        }
    }
}
