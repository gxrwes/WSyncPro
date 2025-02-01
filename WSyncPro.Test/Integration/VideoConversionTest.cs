using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using WSyncPro.Core.Services.VideoProbing;

namespace WSyncPro.Test.Integration
{
    [TestFixture]
    public class VideoConversionTest
    {
        private string _testDirectory;
        private string _demoFilePath;
        private string _convertedFilePath;
        private VideoConverter _videoConverter;

        [SetUp]
        public void SetUp()
        {
            // Create a unique temporary directory for testing.
            _testDirectory = Path.Combine(Path.GetTempPath(), "VideoConversionTest_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDirectory);

            // Locate the demo video file in the Resources folder.
            var demoSourcePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "demo.mp4");
            Assert.IsTrue(File.Exists(demoSourcePath), $"Demo video file not found at: {demoSourcePath}");

            // Copy the demo video file into our temporary test directory.
            _demoFilePath = Path.Combine(_testDirectory, "demo.mp4");
            File.Copy(demoSourcePath, _demoFilePath, true);

            // Initialize the video converter.
            _videoConverter = new VideoConverter();
        }

        [TearDown]
        public void TearDown()
        {
            // Delete the converted file if it exists.
            if (!string.IsNullOrEmpty(_convertedFilePath) && File.Exists(_convertedFilePath))
            {
                try
                {
                    File.Delete(_convertedFilePath);
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Failed to delete converted file: {_convertedFilePath}. Exception: {ex.Message}");
                }
            }

            // Clean up the temporary test directory.
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Test]
        public async Task TestVideoConversionCreatesOutputFile()
        {
            // Arrange: set conversion parameters.
            var parameters = new VideoConversionParameters
            {
                OutputFormat = "mp4",
                VideoCodec = "libx264",
                AudioCodec = "aac",
                // You can add more parameters (e.g., VideoBitRate, FrameRate, etc.) as needed.
            };

            // Act: Convert the video file.
            try
            {
                await _videoConverter.ConvertVideo(_demoFilePath, parameters);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Video conversion failed with exception: {ex}");
            }

            // The output file is named: {originalFileName}_converted.{OutputFormat}
            _convertedFilePath = Path.Combine(Path.GetDirectoryName(_demoFilePath)!, $"{Path.GetFileNameWithoutExtension(_demoFilePath)}_converted.{parameters.OutputFormat}");
            Assert.IsTrue(File.Exists(_convertedFilePath), $"Converted video file was not created at the expected path: {_convertedFilePath}");

            // Verify that the converted file is not empty.
            FileInfo fi = new FileInfo(_convertedFilePath);
            Assert.Greater(fi.Length, 0, "The converted video file is empty.");
        }
    }
}
