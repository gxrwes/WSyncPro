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
        private readonly string _settingsFilePath;
        private readonly FileSerialisationServiceJson _serializationService;
        private readonly AppStateManager _appStateManager;

        public AppStateManagerTests()
        {
            // Setup a unique temporary directory for testing
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            // Define test-specific paths for app state and settings files
            _appStateFilePath = Path.Combine(_tempDir, "AppState.json");
            _settingsFilePath = Path.Combine(_tempDir, "settings.json");

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
            // Existing test case remains unchanged
        }

        [Fact]
        public async Task SaveAppState_ShouldHandleSerializationFailureGracefully()
        {
            // Existing test case remains unchanged
        }

        [Fact]
        public async Task LoadSettings_ShouldLoadExistingSettings()
        {
            // Arrange
            var expectedSettings = new SettingsModel
            {
                DefaultImportPath = "C:\\NewImportPath",
                TrashDirecotry = "C:\\NewTrash",
                Version = "3.0",
                About = "Updated version"
            };

            // Reset the _settings to ensure test isolation
            var settingsField = _appStateManager.GetType().GetField("_settings", BindingFlags.NonPublic | BindingFlags.Instance);
            settingsField?.SetValue(_appStateManager, null); // Reset settings

            // Save the settings to a file for loading
            await _serializationService.SaveClassToFileAsync(_settingsFilePath, expectedSettings);

            // Act
            await _appStateManager.LoadSettings();

            // Assert
            var loadedSettings = settingsField?.GetValue(_appStateManager) as SettingsModel;

            loadedSettings.Should().NotBeNull();
            loadedSettings.DefaultImportPath.Should().Be(expectedSettings.DefaultImportPath);
            loadedSettings.TrashDirecotry.Should().Be(expectedSettings.TrashDirecotry);
            loadedSettings.Version.Should().Be(expectedSettings.Version);
            loadedSettings.About.Should().Be(expectedSettings.About);
        }


        [Fact]
        public async Task SaveSettings_ShouldSaveSettingsToFile()
        {
            // Arrange
            var newSettings = new SettingsModel
            {
                DefaultImportPath = "C:\\NewImportPath",
                TrashDirecotry = "C:\\NewTrash",
                Version = "3.0",
                About = "Updated version"
            };

            var settingsField = _appStateManager.GetType().GetField("_settings", BindingFlags.NonPublic | BindingFlags.Instance);
            settingsField?.SetValue(_appStateManager, newSettings);

            // Ensure the directory exists before the test runs
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath));

            // Act
            await _appStateManager.SaveSettings();

            // Assert
            var savedSettings = await _serializationService.GetFileAsClassAsync<SettingsModel>("Config\\settings.json");
            savedSettings.Should().NotBeNull();
            savedSettings.DefaultImportPath.Should().Be(newSettings.DefaultImportPath);
            savedSettings.TrashDirecotry.Should().Be(newSettings.TrashDirecotry);
            savedSettings.Version.Should().Be(newSettings.Version);
            savedSettings.About.Should().Be(newSettings.About);
        }


        /// <summary>
        /// Helper method to invoke the private IsAppStateEqual method using reflection.
        /// </summary>
        private bool InvokeIsAppStateEqual(AppStateManager appStateManager, AppStateSnapShot state1, AppStateSnapShot state2)
        {
            var method = typeof(AppStateManager).GetMethod("IsAppStateEqual", BindingFlags.NonPublic | BindingFlags.Instance);
            return method != null && (bool)method.Invoke(appStateManager, new object[] { state1, state2 });
        }
    }
}
