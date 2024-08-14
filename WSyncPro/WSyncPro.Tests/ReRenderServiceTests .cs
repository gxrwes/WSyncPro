using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.State;
using WSyncPro.Core.Services.ReRenderService;
using Xunit;

namespace WSyncPro.Tests
{
    public class ReRenderServiceTests : IDisposable
    {
        private readonly ReRenderService _reRenderService;
        private readonly string _testDirectory;
        private readonly string _handBrakeCliPath;
        private readonly string _testVideoFilePath;

        public ReRenderServiceTests()
        {
            // Set the HandBrakeCLI path to the expected location
            _handBrakeCliPath = Path.Combine(Environment.GetEnvironmentVariable("DEVD"), @"Libs\HandBrakeCLI-1.8.2-win-x86_64\HandBrakeCLI.exe");

            // Set the test video file path
            _testVideoFilePath = @"C:\DEVD\Repos\WSyncPro\TestFiles\testvideo.mp4";

            // Skip all tests if the HandBrakeCLI executable or the test video file is not available
            if (!File.Exists(_handBrakeCliPath) || !File.Exists(_testVideoFilePath))
            {
                _testDirectory = null;
                return;
            }

            StateManager.Instance.HandBrakeCliPath = _handBrakeCliPath;

            _reRenderService = new ReRenderService();

            // Setup a temporary directory for testing
            _testDirectory = Path.Combine(Path.GetTempPath(), "ReRenderServiceTest");
            Directory.CreateDirectory(_testDirectory);

            // Copy the test video file to the test directory
            File.Copy(_testVideoFilePath, Path.Combine(_testDirectory, "video1.mp4"), overwrite: true);
        }

        [Fact]
        public void ExecuteReRender_ThrowsException_WhenHandBrakeCliPathIsInvalid()
        {
            if (_testDirectory == null)
            {
                LogSkip("Skipping test due to missing HandBrakeCLI executable or test video file.");
                return; // Skip test
            }

            // Arrange
            StateManager.Instance.HandBrakeCliPath = "InvalidPath";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new ReRenderService());
        }

        [Fact]
        public void ExecuteReRender_SkipsFiles_ContainingPresetInName()
        {
            if (_testDirectory == null)
            {
                LogSkip("Skipping test due to missing HandBrakeCLI executable or test video file.");
                return; // Skip test
            }

            string presetString = "Fast1080p30";

            // Arrange
            var job = new Job
            {
                ReRenderOptions = new ReRenderOptions
                {
                    Preset = (HandbreakPreset)Enum.Parse(typeof(HandbreakPreset), presetString),
                    AdvancedOptions = ""
                }
            };

            var directory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WVideoFile { Name = "video1_Fast1080p30.mp4", Path = Path.Combine(_testDirectory, "video1_Fast1080p30.mp4") }
                }
            };

            // Act
            _reRenderService.ExecuteReRender(job, directory);

            // Assert
            // Ensure that the file was not processed since it already contains the preset
            Assert.False(File.Exists(Path.Combine(_testDirectory, "video1_HB_Fast1080p30.mp4")));
        }

        [Fact]
        public void ExecuteReRender_RendersFileWithCorrectFilename()
        {
            if (_testDirectory == null)
            {
                LogSkip("Skipping test due to missing HandBrakeCLI executable or test video file.");
                return; // Skip test
            }

            // Arrange
            var job = new Job
            {
                ReRenderOptions = new ReRenderOptions
                {
                    Preset = (HandbreakPreset)Enum.Parse(typeof(HandbreakPreset), "Fast1080p30"),
                    AdvancedOptions = ""
                }
            };

            var directory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WVideoFile { Name = "video1.mp4", Path = Path.Combine(_testDirectory, "video1.mp4") }
                }
            };

            // Act
            _reRenderService.ExecuteReRender(job, directory);

            // Assert
            string expectedOutputFile = Path.Combine(_testDirectory, "video1_HB_Fast1080p30.mp4");
            Assert.True(File.Exists(expectedOutputFile), $"Expected file {expectedOutputFile} does not exist.");
        }

        private void LogSkip(string message)
        {
            Console.WriteLine(message);
            // Optionally log to a file or other logging infrastructure here
        }

        public void Dispose()
        {
            // Skip cleanup if the tests were skipped
            if (_testDirectory == null)
            {
                return;
            }

            // Clean up the test directory after each test
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
