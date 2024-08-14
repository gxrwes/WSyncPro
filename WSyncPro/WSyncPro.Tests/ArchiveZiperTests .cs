using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ArchiveServices;
using Xunit;

namespace WSyncPro.Tests
{
    public class ArchiveZiperTests : IDisposable
    {
        private string _testDirectory;
        private string _testSubDirectory;
        private string _targetDirectory;
        private string _zipFileName;

        private void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ArchiveZiperTestDir");
            _testSubDirectory = Path.Combine(_testDirectory, "SubFolder");
            _targetDirectory = Path.Combine(Path.GetTempPath(), "ArchiveZiperTestTargetDir");
            _zipFileName = "TestArchive.zip";

            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_testSubDirectory);

            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "This is a test file.");
            File.WriteAllText(Path.Combine(_testDirectory, "file2.txt"), "This is another test file.");
            File.WriteAllText(Path.Combine(_testSubDirectory, "subfile1.txt"), "This is a subdirectory test file.");
        }

        [Fact]
        public void CreateZipArchive_ThrowsIOException_WhenFileExistsAndOverwriteIsFalse()
        {
            Setup();

            var wDirectory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testDirectory, "file1.txt") },
                    new WFile { Name = "file2.txt", Path = Path.Combine(_testDirectory, "file2.txt") }
                }
            };

            var archiveZiper = new ArchiveZiper();

            // Create the zip file first
            archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: true);

            // Act & Assert
            Assert.Throws<IOException>(() =>
                archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: false));
        }

        [Fact]
        public void CreateZipArchive_CreatesFile_WhenOverwriteIsFalseAndFileDoesNotExist()
        {
            Setup();

            var wDirectory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testDirectory, "file1.txt") },
                    new WFile { Name = "file2.txt", Path = Path.Combine(_testDirectory, "file2.txt") }
                }
            };

            var archiveZiper = new ArchiveZiper();

            // Act
            archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: false);

            // Assert
            string zipFilePath = Path.Combine(_targetDirectory, _zipFileName);
            Assert.True(File.Exists(zipFilePath), "Zip file was not created.");
        }

        [Fact]
        public void CreateZipArchive_OverwritesFile_WhenFileExistsAndOverwriteIsTrue()
        {
            Setup();

            var wDirectory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testDirectory, "file1.txt") },
                    new WFile { Name = "file2.txt", Path = Path.Combine(_testDirectory, "file2.txt") }
                }
            };

            var archiveZiper = new ArchiveZiper();

            // Create the zip file first
            archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: true);

            // Overwrite the zip file
            archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: true);

            // Assert
            string zipFilePath = Path.Combine(_targetDirectory, _zipFileName);
            Assert.True(File.Exists(zipFilePath), "Zip file was not overwritten as expected.");
        }

        [Fact]
        public void CreateZipArchive_CreatesFile_WhenOverwriteIsTrueAndFileDoesNotExist()
        {
            Setup();

            var wDirectory = new WDirectory
            {
                Path = _testDirectory,
                Files = new System.Collections.Generic.List<WFile>
                {
                    new WFile { Name = "file1.txt", Path = Path.Combine(_testDirectory, "file1.txt") },
                    new WFile { Name = "file2.txt", Path = Path.Combine(_testDirectory, "file2.txt") }
                }
            };

            var archiveZiper = new ArchiveZiper();

            // Act
            archiveZiper.CreateZipArchive(wDirectory, _targetDirectory, _zipFileName, overwrite: true);

            // Assert
            string zipFilePath = Path.Combine(_targetDirectory, _zipFileName);
            Assert.True(File.Exists(zipFilePath), "Zip file was not created.");
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }

            if (Directory.Exists(_targetDirectory))
            {
                Directory.Delete(_targetDirectory, true);
            }
        }
    }
}
