using System;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ArchiveServices;
using Xunit;

namespace WSyncPro.Tests
{
    public class ArchiveFunctionReplacerTests
    {
        [Fact]
        public void DirectoryStructureReplacer_WithValidInput_ReplacesCorrectly()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var file = new WFile
            {
                id = 456,
                Name = "testfile.txt"
            };
            string directoryStructure = "YEAR\\MONTH\\DAY\\JOBID\\ITEMID\\JOBNAME\\FILENAME";

            // Act
            var result = ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, file);

            // Assert
            var expected = $"{DateTime.Now.Year}\\{DateTime.Now.Month:D2}\\{DateTime.Now.Day:D2}\\123\\456\\TestJob\\testfile.txt";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DirectoryStructureReplacer_MissingJobId_ThrowsArgumentException()
        {
            // Arrange
            var job = new Job
            {
                Name = "TestJob"
            };
            var file = new WFile
            {
                id = 456,
                Name = "testfile.txt"
            };
            string directoryStructure = "JOBID\\ITEMID\\JOBNAME\\FILENAME";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, file));

            Assert.Equal("Job ID is required but was not provided.", exception.Message);
        }

        [Fact]
        public void DirectoryStructureReplacer_MissingFileId_ThrowsArgumentException()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var file = new WFile
            {
                Name = "testfile.txt"
            };
            string directoryStructure = "ITEMID\\JOBNAME\\FILENAME";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, file));

            Assert.Equal("File ID is required but was not provided or is invalid.", exception.Message);
        }

        [Fact]
        public void DirectoryStructureReplacer_WithVideoFile_ResolvesResolution()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var videoFile = new WVideoFile
            {
                id = 456,
                Name = "testvideo.mp4",
                Resolution = Resolution.HD1080
            };
            string directoryStructure = "JOBID\\ITEMID\\JOBNAME\\FILENAME\\RESOLUTION";

            // Act
            var result = ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, videoFile);

            // Assert
            var expected = $"123\\456\\TestJob\\testvideo.mp4\\HD1080";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DirectoryStructureReplacer_WithCustomResolution_ThrowsArgumentException()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var videoFile = new WVideoFile
            {
                id = 456,
                Name = "testvideo.mp4",
                Resolution = Resolution.CUSTOM
            };
            string directoryStructure = "JOBID\\ITEMID\\JOBNAME\\FILENAME\\RESOLUTION";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, videoFile));

            Assert.Equal("Video file resolution is set to CUSTOM, which is not allowed.", exception.Message);
        }

        [Fact]
        public void DirectoryStructureReplacer_NullOrEmptyDirectoryStructure_ThrowsArgumentNullException()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var file = new WFile
            {
                id = 456,
                Name = "testfile.txt"
            };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ArchiveFunctionReplacer.DirectoryStructureReplacer(null, job, file));

            Assert.Throws<ArgumentNullException>(() =>
                ArchiveFunctionReplacer.DirectoryStructureReplacer("", job, file));
        }

        [Fact]
        public void DirectoryStructureReplacer_WithCustomPlaceholder_HandlesCustomLogic()
        {
            // Arrange
            var job = new Job
            {
                Id = "123",
                Name = "TestJob"
            };
            var file = new WFile
            {
                id = 456,
                Name = "testfile.txt"
            };
            string directoryStructure = "CUSTOM\\JOBID\\ITEMID";

            // Act
            var result = ArchiveFunctionReplacer.DirectoryStructureReplacer(directoryStructure, job, file);

            // Assert
            var expected = $"CustomValue\\123\\456"; // Assuming CustomValue is the placeholder replacement
            Assert.Equal(expected, result);
        }
    }
}
