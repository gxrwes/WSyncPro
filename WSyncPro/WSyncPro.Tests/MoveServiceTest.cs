using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services;
using Xunit;

namespace WSyncPro.Tests
{
    public class MoveServiceTests : IDisposable
    {
        private string _testSourceDirectory;
        private string _testTargetDirectory;

        public MoveServiceTests()
        {
            _testSourceDirectory = Path.Combine(Path.GetTempPath(), "MoveServiceTestSource");
            _testTargetDirectory = Path.Combine(Path.GetTempPath(), "MoveServiceTestTarget");

            Directory.CreateDirectory(_testSourceDirectory);
            Directory.CreateDirectory(_testTargetDirectory);

            File.WriteAllText(Path.Combine(_testSourceDirectory, "file1.txt"), "This is a test file.");
            File.WriteAllText(Path.Combine(_testSourceDirectory, "file2.txt"), "This is another test file.");

            string subDirectory = Path.Combine(_testSourceDirectory, "SubFolder");
            Directory.CreateDirectory(subDirectory);
            File.WriteAllText(Path.Combine(subDirectory, "subfile1.txt"), "This is a subdirectory test file.");
        }

        [Fact]
        public void MoveFiles_AlwaysOverwrite_MovesAllFiles()
        {
            var sourceDirectory = new WDirectory
            {
                Path = _testSourceDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testSourceDirectory, "file1.txt") },
                    new WFile { Name = "file2.txt", Path = Path.Combine(_testSourceDirectory, "file2.txt") },
                    new WDirectory
                    {
                        Name = "SubFolder",
                        Path = Path.Combine(_testSourceDirectory, "SubFolder"),
                        Files = new System.Collections.Generic.List<WFile>
                        {
                            new WFile { Name = "subfile1.txt", Path = Path.Combine(_testSourceDirectory, "SubFolder", "subfile1.txt") }
                        }
                    }
                }
            };

            var job = new Job
            {
                TargetDirectory = _testTargetDirectory
            };

            var moveService = new MoveService();

            // Act
            moveService.MoveFiles(sourceDirectory, job, FileOverwriteOptions.ALWAYS, true);

            // Assert
            Assert.True(File.Exists(Path.Combine(_testTargetDirectory, "file1.txt")));
            Assert.True(File.Exists(Path.Combine(_testTargetDirectory, "file2.txt")));
            Assert.True(File.Exists(Path.Combine(_testTargetDirectory, "SubFolder", "subfile1.txt")));

            Assert.False(File.Exists(Path.Combine(_testSourceDirectory, "file1.txt")));
            Assert.False(File.Exists(Path.Combine(_testSourceDirectory, "file2.txt")));
            Assert.False(File.Exists(Path.Combine(_testSourceDirectory, "SubFolder", "subfile1.txt")));
        }

        [Fact]
        public void MoveFiles_NeverOverwrite_SkipsExistingFiles()
        {
            File.WriteAllText(Path.Combine(_testTargetDirectory, "file1.txt"), "Existing file content.");

            var sourceDirectory = new WDirectory
            {
                Path = _testSourceDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testSourceDirectory, "file1.txt") }
                }
            };

            var job = new Job
            {
                TargetDirectory = _testTargetDirectory
            };

            var moveService = new MoveService();

            // Act
            moveService.MoveFiles(sourceDirectory, job, FileOverwriteOptions.NEVER, false);

            // Assert
            string content = File.ReadAllText(Path.Combine(_testTargetDirectory, "file1.txt"));
            Assert.Equal("Existing file content.", content);
            Assert.True(File.Exists(Path.Combine(_testSourceDirectory, "file1.txt")));
        }

        [Fact]
        public void MoveFiles_NewerOverwrite_OverwritesWithNewerFiles()
        {
            string targetFilePath = Path.Combine(_testTargetDirectory, "file1.txt");
            File.WriteAllText(targetFilePath, "Older file content.");
            File.SetLastWriteTime(targetFilePath, DateTime.Now.AddDays(-1));

            var sourceDirectory = new WDirectory
            {
                Path = _testSourceDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testSourceDirectory, "file1.txt") }
                }
            };

            var job = new Job
            {
                TargetDirectory = _testTargetDirectory
            };

            var moveService = new MoveService();

            // Act
            moveService.MoveFiles(sourceDirectory, job, FileOverwriteOptions.NEWER, false);

            // Assert
            string content = File.ReadAllText(targetFilePath);
            Assert.Equal("This is a test file.", content);
            Assert.False(File.Exists(Path.Combine(_testSourceDirectory, "file1.txt")));
        }

        public void Dispose()
        {
            if (Directory.Exists(_testSourceDirectory))
            {
                Directory.Delete(_testSourceDirectory, true);
            }

            if (Directory.Exists(_testTargetDirectory))
            {
                Directory.Delete(_testTargetDirectory, true);
            }
        }
    }
}
