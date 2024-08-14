using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace WSyncPro.Core.Test
{
    [TestClass]
    public class DirectoryScannerTests
    {
        private DirectoryScanner _scanner;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the DirectoryScanner before each test
            _scanner = new DirectoryScanner();
        }

        [TestMethod]
        public void ScanDirectory_DirectoryNotFound_ThrowsException()
        {
            // Arrange
            var nonExistentPath = @"C:\NonExistentDirectory";

            // Act & Assert
            Assert.ThrowsException<DirectoryNotFoundException>(() => _scanner.ScanDirectory(nonExistentPath));
        }

        [TestMethod]
        public void ScanDirectory_EmptyDirectory_ReturnsEmptyWDirectory()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "EmptyTestDirectory");
            Directory.CreateDirectory(tempPath);

            // Act
            var result = _scanner.ScanDirectory(tempPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Files.Count);

            // Cleanup
            Directory.Delete(tempPath);
        }

        [TestMethod]
        public void ScanDirectory_WithFiles_ReturnsCorrectWFiles()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryWithFiles");
            Directory.CreateDirectory(tempPath);

            var filePath1 = Path.Combine(tempPath, "test.jpg");
            var filePath2 = Path.Combine(tempPath, "test.mp3");
            File.Create(filePath1).Dispose();  // Create a test image file
            File.Create(filePath2).Dispose();  // Create a test audio file

            // Act
            var result = _scanner.ScanDirectory(tempPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Files.Count);

            var imageFile = result.Files[0] as WImageFile;
            var audioFile = result.Files[1] as WAudioFile;

            Assert.IsNotNull(imageFile);
            Assert.AreEqual("test.jpg", imageFile.Name);

            Assert.IsNotNull(audioFile);
            Assert.AreEqual("test.mp3", audioFile.Name);

            // Cleanup
            Directory.Delete(tempPath, true);
        }

        [TestMethod]
        public void ScanDirectory_WithSubdirectories_ReturnsCorrectStructure()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryWithSubdirectories");
            Directory.CreateDirectory(tempPath);

            var subDirPath = Path.Combine(tempPath, "SubDirectory");
            Directory.CreateDirectory(subDirPath);

            var filePath = Path.Combine(subDirPath, "test.mp4");
            File.Create(filePath).Dispose();  // Create a test video file

            // Act
            var result = _scanner.ScanDirectory(tempPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Files.Count); // Only one subdirectory

            var subDirectory = result.Files[0] as WDirectory;
            Assert.IsNotNull(subDirectory);
            Assert.AreEqual("SubDirectory", subDirectory.Name);
            Assert.AreEqual(1, subDirectory.Files.Count); // One file in the subdirectory

            var videoFile = subDirectory.Files[0] as WVideoFile;
            Assert.IsNotNull(videoFile);
            Assert.AreEqual("test.mp4", videoFile.Name);

            // Cleanup
            Directory.Delete(tempPath, true);
        }
    }
}
