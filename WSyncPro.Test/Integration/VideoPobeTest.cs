using NUnit.Framework;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WSyncPro.Core.Services.VideoProbing;
using WSyncPro.Models.Files;

namespace WSyncPro.Test.Integration
{
    [TestFixture]
    internal class VideoPobeTest
    {
        private string _testDirectory;
        private string _demoFilePath;
        private string _metadataFilePath;
        private VideoProbeService _videoProbeService;

        [SetUp]
        public void SetUp()
        {
            // Create a unique temporary directory for testing.
            _testDirectory = Path.Combine(Path.GetTempPath(), "VideoProbeTest_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDirectory);

            // Locate the demo video file in the Resources folder.
            var demoSourcePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "demo.mp4");
            Assert.IsTrue(File.Exists(demoSourcePath), $"Demo video file not found at: {demoSourcePath}");

            // Copy the demo video file into our temporary test directory.
            _demoFilePath = Path.Combine(_testDirectory, "demo.mp4");
            File.Copy(demoSourcePath, _demoFilePath, true);

            // Initialize the video probe service.
            _videoProbeService = new VideoProbeService();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the temporary test directory.
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Test]
        public async Task TestVideoProbingCreatesMetadataFile()
        {
            // Run the probe service on the test directory.
            try
            {
                await _videoProbeService.RunInfoProbe(_testDirectory);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Video probing failed with exception: {ex}");
            }

            // Expect a metadata file with the same name plus ".metainf"
            _metadataFilePath = _demoFilePath + ".metainf";
            Assert.IsTrue(File.Exists(_metadataFilePath), "Metadata file was not created for the video file.");

            // Read and deserialize the JSON content.
            string jsonContent = File.ReadAllText(_metadataFilePath);
            FileInfoPayload payload = null;
            try
            {
                payload = JsonSerializer.Deserialize<FileInfoPayload>(jsonContent);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to deserialize metadata file content: {ex}");
            }

            Assert.IsNotNull(payload, "Deserialized metadata payload is null.");
            Assert.AreEqual("demo.mp4", payload.FileName, "The file name in metadata does not match the expected value.");
            Assert.Greater(payload.DurationSeconds, 0, "The video duration should be greater than 0 seconds.");
            // Add additional assertions here if needed (e.g. width, height, codec names)
        }
    }
}
