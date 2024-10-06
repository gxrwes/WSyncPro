using System;
using System.IO;
using System.Threading.Tasks;
using WSyncPro.Util.Services;
using Xunit;

namespace WSyncPro.Test.Unit.Util
{
    public class FileCopyMoveServiceUnitTest : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _srcFilePath;
        private readonly string _destFilePath;
        private readonly FileCopyMoveService _service;

        public FileCopyMoveServiceUnitTest()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _srcFilePath = Path.Combine(_testDirectory, "testFile.txt");
            _destFilePath = Path.Combine(_testDirectory, "testFileCopied.txt");

            File.WriteAllText(_srcFilePath, "Test Content");

            _service = new FileCopyMoveService();
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task CopyFileAsync_ShouldCopyFileSuccessfully()
        {
            // Act
            await _service.CopyFileAsync(_srcFilePath, _destFilePath);

            // Assert
            Assert.True(File.Exists(_destFilePath));
            Assert.Equal(File.ReadAllText(_srcFilePath), File.ReadAllText(_destFilePath));
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task MoveFileAsync_ShouldMoveFileSuccessfully()
        {
            // Act
            await _service.MoveFileAsync(_srcFilePath, _destFilePath);

            // Assert
            Assert.False(File.Exists(_srcFilePath));
            Assert.True(File.Exists(_destFilePath));
            Assert.Equal("Test Content", File.ReadAllText(_destFilePath));
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task CopyFileAsync_ShouldThrowException_WhenSourceFileDoesNotExist()
        {
            // Arrange
            var nonExistentFilePath = Path.Combine(_testDirectory, "nonExistent.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _service.CopyFileAsync(nonExistentFilePath, _destFilePath));
        }

        [Fact]
        [Trait("Category", "Unit Test")]
        public async Task MoveFileAsync_ShouldThrowException_WhenSourceFileDoesNotExist()
        {
            // Arrange
            var nonExistentFilePath = Path.Combine(_testDirectory, "nonExistent.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _service.MoveFileAsync(nonExistentFilePath, _destFilePath));
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
