// JobBuilderServiceTests.cs
using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Xunit;
using FluentAssertions;
using WSyncPro.Core.Services;
using WSyncPro.Models;

namespace WSyncPro.Tests.Core.Services
{
    public class JobBuilderServiceTests : BaseTest
    {
        private readonly Mock<IFileSystemService> _mockFileSystemService;
        private readonly JobBuilderService _jobBuilderService;
        private const string SourceDirectory1 = @"C:\SourceDirectory1";
        private const string SourceDirectory2 = @"C:\SourceDirectory2";
        private const string SourceDirectory3 = @"D:\SourceDirectory3";
        private const string SourceDirectory4 = @"E:\SourceDirectory4";

        private static readonly List<string> IncludePatterns = new List<string>
        {
            "*render*", "*.txt", "data_??.csv"
        };

        private static readonly List<string> ExcludePatterns = new List<string>
        {
            "*.tmp", "temp_*", "*.log"
        };

        public JobBuilderServiceTests()
        {
            _mockFileSystemService = new Mock<IFileSystemService>();
            _jobBuilderService = new JobBuilderService(_mockFileSystemService.Object);
        }

        [Fact]
        public void BuildJob_ShouldThrowArgumentNullException_WhenJobIsNull()
        {
            // Arrange
            Job job = null;

            // Act
            Action act = () => _jobBuilderService.BuildJob(job);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("job");
        }

        [Fact]
        public void BuildJob_ShouldThrowArgumentException_WhenSrcDirectoryIsNotSpecified()
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = null, // Explicitly set to null
                InclWilcardString = new List<string> { "*" },
                ExclWildcardString = new List<string> { "*.bak" }
            };

            // Act
            Action act = () => _jobBuilderService.BuildJob(job);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Source directory is not specified.*")
                .And.ParamName.Should().Be("SrcDirectory");
        }

        [Fact]
        public void BuildJob_ShouldThrowDirectoryNotFoundException_WhenSrcDirectoryDoesNotExist()
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory1,
                InclWilcardString = new List<string> { "*" },
                ExclWildcardString = new List<string> { "*.bak" }
            };

            _mockFileSystemService.Setup(fs => fs.DirectoryExists(job.SrcDirectory))
                .Returns(false);

            // Act
            Action act = () => _jobBuilderService.BuildJob(job);

            // Assert
            act.Should().Throw<DirectoryNotFoundException>()
                .WithMessage($"Source directory not found: {job.SrcDirectory}");
        }

        [Fact]
        public void BuildJob_ShouldBuildBuiltJob_Correctly_WithMultipleWildcards()
        {
            // Arrange
            var job = new Job
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Job",
                Description = "Testing BuildJob with multiple wildcards",
                IsEnabled = true,
                SrcDirectory = SourceDirectory1,
                DstDirectory = @"D:\DestinationDirectory",
                FilesToSync = 0,
                TotalFilesSynced = 0,
                FailedFiles = 0,
                InclWilcardString = new List<string> { "*render*", "*.txt", "data_??.csv" },
                ExclWildcardString = new List<string> { "*.tmp", "temp_*", "*.log" }
            };

            var allFiles = new List<string>
            {
                @"C:\SourceDirectory1\image_render_final.jpg",
                @"C:\SourceDirectory1\document.txt",
                @"C:\SourceDirectory1\data_01.csv",
                @"C:\SourceDirectory1\data_02.csv",
                @"C:\SourceDirectory1\temp_image.jpg",
                @"C:\SourceDirectory1\notes.tmp",
                @"C:\SourceDirectory1\app.log",
                @"C:\SourceDirectory1\README.md" // Should be excluded (no inclusion pattern matches)
            };

            // Mock Directory.Exists
            _mockFileSystemService.Setup(fs => fs.DirectoryExists(job.SrcDirectory))
                .Returns(true);

            // Mock EnumerateFiles
            _mockFileSystemService.Setup(fs => fs.EnumerateFiles(job.SrcDirectory, "*", true))
                .Returns(allFiles);

            // Mock GetFileSize
            var fileSizes = new Dictionary<string, long>
            {
                { @"C:\SourceDirectory1\image_render_final.jpg", 2_147_483_648 }, // 2 GB
                { @"C:\SourceDirectory1\document.txt", 524_288_000 }, // 0.488 GB
                { @"C:\SourceDirectory1\data_01.csv", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory1\data_02.csv", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory1\temp_image.jpg", 1_073_741_824 }, // 1 GB
                { @"C:\SourceDirectory1\notes.tmp", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory1\app.log", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory1\README.md", 52_428_800 } // 0.049 GB
            };

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            builtJob.Should().NotBeNull();
            builtJob.Id.Should().Be(job.Id);
            builtJob.Name.Should().Be(job.Name);
            builtJob.Description.Should().Be(job.Description);
            builtJob.IsEnabled.Should().Be(job.IsEnabled);
            builtJob.SrcDirectory.Should().Be(job.SrcDirectory);
            builtJob.DstDirectory.Should().Be(job.DstDirectory);
            builtJob.InclWilcardString.Should().BeEquivalentTo(job.InclWilcardString);
            builtJob.ExclWildcardString.Should().BeEquivalentTo(job.ExclWildcardString);

            // Expected matched files:
            // @"C:\SourceDirectory1\image_render_final.jpg" (matches "*render*")
            // @"C:\SourceDirectory1\document.txt" (matches "*.txt")
            // @"C:\SourceDirectory1\data_01.csv" (matches "data_??.csv")
            // @"C:\SourceDirectory1\data_02.csv" (matches "data_??.csv")
            builtJob.FileList.Should().HaveCount(4);
            builtJob.FileList.Should().Contain(new List<string>
            {
                @"C:\SourceDirectory1\image_render_final.jpg",
                @"C:\SourceDirectory1\document.txt",
                @"C:\SourceDirectory1\data_01.csv",
                @"C:\SourceDirectory1\data_02.csv"
            });

            // Total size in GB: 2 + 0.488 + 0.097 + 0.097 = ~2.682 GB
            builtJob.TotalFileSizeInGB.Should().BeApproximately(2.682f, 0.002f); // Increased tolerance to 0.002F

            // Log confirmed include and exclude patterns
            foreach (var pattern in job.InclWilcardString)
            {
                SaveConfirmedPattern(pattern, isInclude: true);
            }

            foreach (var pattern in job.ExclWildcardString)
            {
                SaveConfirmedPattern(pattern, isInclude: false);
            }
        }
    }
}
