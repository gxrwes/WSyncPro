using System;
using System.IO;
using System.Linq;
using WSyncPro.Core.Services.ScanService;
using WSyncPro.Core.Models.FileModels;
using Xunit;

namespace WSyncPro.Tests
{
    public class DirectoryScannerTests
    {
        private readonly DirectoryScanner _scanner;

        public DirectoryScannerTests()
        {
            _scanner = new DirectoryScanner();
        }

        [Fact]
        public void ScanDirectory_DirectoryNotFound_ThrowsException()
        {
            // Arrange
            var nonExistentPath = @"C:\NonExistentDirectory";

            // Act & Assert (synchronous method)
            Assert.Throws<DirectoryNotFoundException>(() => _scanner.ScanDirectory(nonExistentPath));
        }

        [Fact]
        public void ScanDirectory_EmptyDirectory_ReturnsEmptyWDirectory()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "EmptyTestDirectory");
            Directory.CreateDirectory(tempPath);

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Files);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath);
            }
        }

        [Fact]
        public void ScanDirectory_WithFiles_ReturnsCorrectWFiles()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryWithFiles");
            Directory.CreateDirectory(tempPath);

            var filePath1 = Path.Combine(tempPath, "test.jpg");
            var filePath2 = Path.Combine(tempPath, "test.mp3");
            File.Create(filePath1).Dispose();  // Create a test image file
            File.Create(filePath2).Dispose();  // Create a test audio file

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Files.Count);

                var imageFile = result.Files.OfType<WImageFile>().FirstOrDefault();
                var audioFile = result.Files.OfType<WAudioFile>().FirstOrDefault();

                Assert.NotNull(imageFile);
                Assert.Equal("test.jpg", imageFile.Name);

                Assert.NotNull(audioFile);
                Assert.Equal("test.mp3", audioFile.Name);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void ScanDirectory_WithSubdirectories_ReturnsCorrectStructure()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryWithSubdirectories");
            Directory.CreateDirectory(tempPath);

            var subDirPath = Path.Combine(tempPath, "SubDirectory");
            Directory.CreateDirectory(subDirPath);

            var filePath = Path.Combine(subDirPath, "test.mp4");
            File.Create(filePath).Dispose();  // Create a test video file

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.Files); // Only one subdirectory

                var subDirectory = result.Files.OfType<WDirectory>().FirstOrDefault();
                Assert.NotNull(subDirectory);
                Assert.Equal("SubDirectory", subDirectory.Name);
                Assert.Single(subDirectory.Files); // One file in the subdirectory

                var videoFile = subDirectory.Files.OfType<WVideoFile>().FirstOrDefault();
                Assert.NotNull(videoFile);
                Assert.Equal("test.mp4", videoFile.Name);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void ScanDirectory_WithUnsupportedFileType_LogsWarning()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryWithUnsupportedFile");
            Directory.CreateDirectory(tempPath);

            var filePath = Path.Combine(tempPath, "unsupported.xyz");
            File.Create(filePath).Dispose();  // Create a test file with unsupported extension

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Files);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void ScanDirectory_MixedContent_ReturnsCorrectHierarchy()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryMixedContent");
            Directory.CreateDirectory(tempPath);

            var subDirPath = Path.Combine(tempPath, "SubDirectory");
            Directory.CreateDirectory(subDirPath);

            var filePath1 = Path.Combine(tempPath, "test.jpg");
            var filePath2 = Path.Combine(subDirPath, "test.mp3");
            File.Create(filePath1).Dispose();  // Create a test image file in root
            File.Create(filePath2).Dispose();  // Create a test audio file in subdirectory

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Files.Count); // 1 file + 1 subdirectory

                var imageFile = result.Files.OfType<WImageFile>().FirstOrDefault();
                Assert.NotNull(imageFile);
                Assert.Equal("test.jpg", imageFile.Name);

                var subDirectory = result.Files.OfType<WDirectory>().FirstOrDefault();
                Assert.NotNull(subDirectory);
                Assert.Equal("SubDirectory", subDirectory.Name);
                Assert.Single(subDirectory.Files); // One file in the subdirectory

                var audioFile = subDirectory.Files.OfType<WAudioFile>().FirstOrDefault();
                Assert.NotNull(audioFile);
                Assert.Equal("test.mp3", audioFile.Name);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void ScanDirectory_NestedSubdirectories_ReturnsDeepHierarchy()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), "TestDirectoryNestedSubdirectories");
            Directory.CreateDirectory(tempPath);

            var level1Path = Path.Combine(tempPath, "Level1");
            Directory.CreateDirectory(level1Path);

            var level2Path = Path.Combine(level1Path, "Level2");
            Directory.CreateDirectory(level2Path);

            var filePath = Path.Combine(level2Path, "test.txt");
            File.Create(filePath).Dispose();  // Create a test text file in the deepest directory

            try
            {
                // Act
                var result = _scanner.ScanDirectory(tempPath);

                // Assert
                Assert.NotNull(result);
                var level1Directory = result.Files.OfType<WDirectory>().FirstOrDefault();
                Assert.NotNull(level1Directory);
                Assert.Equal("Level1", level1Directory.Name);

                var level2Directory = level1Directory.Files.OfType<WDirectory>().FirstOrDefault();
                Assert.NotNull(level2Directory);
                Assert.Equal("Level2", level2Directory.Name);

                var textFile = level2Directory.Files.OfType<WDocumentFile>().FirstOrDefault();
                Assert.NotNull(textFile);
                Assert.Equal("test.txt", textFile.Name);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempPath, true);
            }
        }
    }
}
