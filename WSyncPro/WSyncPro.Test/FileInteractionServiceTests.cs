using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using Xunit;
using FluentAssertions;

namespace WSyncPro.Test
{
    public class FileInteractionServiceTests : IAsyncLifetime
    {
        private readonly string _tempSourceDir;
        private readonly string _tempDestDir;
        private readonly string _tempTrashDir;
        private TestableFileInteractionService _service;

        public FileInteractionServiceTests()
        {
            // Initialize temporary directories with unique GUIDs to avoid conflicts
            _tempSourceDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _tempDestDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _tempTrashDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(_tempSourceDir);
            Directory.CreateDirectory(_tempDestDir);
            Directory.CreateDirectory(_tempTrashDir);
        }

        // This method is called before any tests are run
        public async Task InitializeAsync()
        {
            // Initialize the service and set the TrashFolder asynchronously
            _service = new TestableFileInteractionService(hasEnoughSpace: true);
            await _service.SetTrashFolderAsync(_tempTrashDir);
        }

        // This method is called after all tests have run
        public async Task DisposeAsync()
        {
            // Clean up temporary directories asynchronously
            await Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(_tempSourceDir))
                        Directory.Delete(_tempSourceDir, true);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    Console.WriteLine($"Error deleting source directory: {ex.Message}");
                }

                try
                {
                    if (Directory.Exists(_tempDestDir))
                        Directory.Delete(_tempDestDir, true);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    Console.WriteLine($"Error deleting destination directory: {ex.Message}");
                }

                try
                {
                    if (Directory.Exists(_tempTrashDir))
                        Directory.Delete(_tempTrashDir, true);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    Console.WriteLine($"Error deleting trash directory: {ex.Message}");
                }
            });
        }

        [Fact]
        public async Task CopyFileAsync_ShouldCopyFile_WhenDestinationDoesNotExist()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "file1.txt");
            string dstFile = Path.Combine(_tempDestDir, "file1.txt");
            File.WriteAllText(srcFile, "Hello World");

            // Act
            await _service.CopyFileAsync(srcFile, dstFile);

            // Assert
            File.Exists(dstFile).Should().BeTrue();
            File.ReadAllText(dstFile).Should().Be("Hello World");
        }

        [Fact]
        public async Task CopyFileAsync_ShouldNotCopyFile_WhenDestinationExists_AndForceIsFalse()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "file2.txt");
            string dstFile = Path.Combine(_tempDestDir, "file2.txt");
            File.WriteAllText(srcFile, "Source Content");
            File.WriteAllText(dstFile, "Destination Content");

            // Act
            await _service.CopyFileAsync(srcFile, dstFile, force: false);

            // Assert
            File.ReadAllText(dstFile).Should().Be("Destination Content");
        }

        [Fact]
        public async Task CopyFileAsync_ShouldOverwriteFile_WhenForceIsTrue()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "file3.txt");
            string dstFile = Path.Combine(_tempDestDir, "file3.txt");
            File.WriteAllText(srcFile, "Source Content");
            File.WriteAllText(dstFile, "Destination Content");

            // Act
            await _service.CopyFileAsync(srcFile, dstFile, force: true);

            // Assert
            File.ReadAllText(dstFile).Should().Be("Source Content");
        }

        [Fact]
        public async Task CopyFileAsync_ShouldCopy_WhenIfNewerIsTrue_AndSourceIsNewer()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "file4.txt");
            string dstFile = Path.Combine(_tempDestDir, "file4.txt");
            File.WriteAllText(srcFile, "Newer Content");
            File.WriteAllText(dstFile, "Older Content");

            // Modify the last write time to make source newer
            File.SetLastWriteTime(srcFile, DateTime.Now);
            File.SetLastWriteTime(dstFile, DateTime.Now.AddMinutes(-10));

            // Act
            await _service.CopyFileAsync(srcFile, dstFile, ifNewer: true);

            // Assert
            File.ReadAllText(dstFile).Should().Be("Newer Content");
        }

        [Fact]
        public async Task CopyFileAsync_ShouldNotCopy_WhenIfNewerIsTrue_AndSourceIsOlder()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "file5.txt");
            string dstFile = Path.Combine(_tempDestDir, "file5.txt");
            File.WriteAllText(srcFile, "Older Content");
            File.WriteAllText(dstFile, "Newer Content");

            // Modify the last write time to make source older
            File.SetLastWriteTime(srcFile, DateTime.Now.AddMinutes(-10));
            File.SetLastWriteTime(dstFile, DateTime.Now);

            // Act
            await _service.CopyFileAsync(srcFile, dstFile, ifNewer: true);

            // Assert
            File.ReadAllText(dstFile).Should().Be("Newer Content");
        }

        [Fact]
        public async Task CopyFilesAsync_ShouldCopyAllFiles()
        {
            // Arrange
            var srcFiles = new List<string>
            {
                Path.Combine(_tempSourceDir, "fileA.txt"),
                Path.Combine(_tempSourceDir, "fileB.txt"),
                Path.Combine(_tempSourceDir, "fileC.txt")
            };

            foreach (var file in srcFiles)
            {
                File.WriteAllText(file, $"Content of {Path.GetFileName(file)}");
            }

            // Act
            await _service.CopyFilesAsync(srcFiles, _tempDestDir);

            // Assert
            foreach (var src in srcFiles)
            {
                string dst = Path.Combine(_tempDestDir, Path.GetFileName(src));
                File.Exists(dst).Should().BeTrue();
                File.ReadAllText(dst).Should().Be($"Content of {Path.GetFileName(src)}");
            }
        }

        [Fact]
        public async Task MoveFileAsync_ShouldMoveFileSuccessfully()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "fileMove1.txt");
            string dstFile = Path.Combine(_tempDestDir, "fileMove1.txt");
            File.WriteAllText(srcFile, "Move Content");

            // Act
            await _service.MoveFileAsync(srcFile, dstFile);

            // Assert
            File.Exists(srcFile).Should().BeFalse();
            File.Exists(dstFile).Should().BeTrue();
            File.ReadAllText(dstFile).Should().Be("Move Content");
        }

        [Fact]
        public async Task MoveFilesAsync_ShouldMoveAllFilesSuccessfully()
        {
            // Arrange
            var srcFiles = new List<string>
            {
                Path.Combine(_tempSourceDir, "fileM1.txt"),
                Path.Combine(_tempSourceDir, "fileM2.txt"),
                Path.Combine(_tempSourceDir, "fileM3.txt")
            };

            foreach (var file in srcFiles)
            {
                File.WriteAllText(file, $"Content of {Path.GetFileName(file)}");
            }

            // Act
            await _service.MoveFilesAsync(srcFiles, _tempDestDir);

            // Assert
            foreach (var src in srcFiles)
            {
                string dst = Path.Combine(_tempDestDir, Path.GetFileName(src));
                File.Exists(src).Should().BeFalse();
                File.Exists(dst).Should().BeTrue();
                File.ReadAllText(dst).Should().Be($"Content of {Path.GetFileName(src)}");
            }
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldMoveFileToTrash_WhenEnoughSpace()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "fileDel1.txt");
            File.WriteAllText(srcFile, "Delete Content");

            // Act
            bool result = await _service.DeleteFileAsync(srcFile);

            // Assert
            result.Should().BeTrue();
            File.Exists(srcFile).Should().BeFalse();
            string trashedFile = Path.Combine(_tempTrashDir, "fileDel1.txt");
            File.Exists(trashedFile).Should().BeTrue();
            File.ReadAllText(trashedFile).Should().Be("Delete Content");
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldDeleteFile_WhenNotEnoughSpace()
        {
            // Arrange
            string srcFile = Path.Combine(_tempSourceDir, "fileDel2.txt");
            File.WriteAllText(srcFile, "Content to Delete");

            // Create a derived test class that simulates not enough space
            var testService = new TestableFileInteractionService(hasEnoughSpace: false);
            await testService.SetTrashFolderAsync(_tempTrashDir);

            // Act
            bool result = await testService.DeleteFileAsync(srcFile);

            // Assert
            result.Should().BeTrue();
            File.Exists(srcFile).Should().BeFalse();
            string trashedFile = Path.Combine(_tempTrashDir, "fileDel2.txt");
            File.Exists(trashedFile).Should().BeFalse();
        }

        [Fact]
        public async Task TrashFolder_ShouldBeThreadSafe()
        {
            // Arrange
            int iterations = 100;
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < iterations; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    string newTrashFolder = $"{_tempTrashDir}_{index}";
                    await _service.SetTrashFolderAsync(newTrashFolder);
                    string currentTrash = await _service.GetTrashFolderAsync();
                    // Optionally, perform assertions inside
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            string finalTrashFolder = await _service.GetTrashFolderAsync();
            finalTrashFolder.Should().StartWith(_tempTrashDir);
        }

        [Fact]
        public async Task ConcurrentCopyFiles_ShouldHandleMultipleCopies()
        {
            // Arrange
            var srcFiles = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                string file = Path.Combine(_tempSourceDir, $"concurrentFile_{i}.txt");
                File.WriteAllText(file, $"Content {i}");
                srcFiles.Add(file);
            }

            // Act
            await _service.CopyFilesAsync(srcFiles, _tempDestDir, force: true);

            // Assert
            foreach (var src in srcFiles)
            {
                string dst = Path.Combine(_tempDestDir, Path.GetFileName(src));
                File.Exists(dst).Should().BeTrue();
                // Extract the number from the filename to verify content
                string fileName = Path.GetFileName(src);
                string numberPart = fileName.Split('_')[1].Split('.')[0];
                File.ReadAllText(dst).Should().Be($"Content {numberPart}");
            }
        }

        /// <summary>
        /// A derived class to allow overriding the HasEnoughSpace method for testing.
        /// </summary>
        private class TestableFileInteractionService : FileInteractionService
        {
            private readonly bool _hasEnoughSpaceOverride;

            public TestableFileInteractionService(bool hasEnoughSpace)
                : base()
            {
                _hasEnoughSpaceOverride = hasEnoughSpace;
            }

            // Override the HasEnoughSpace method
            protected override bool HasEnoughSpace(string destinationPath)
            {
                return _hasEnoughSpaceOverride;
            }
        }
    }
}
