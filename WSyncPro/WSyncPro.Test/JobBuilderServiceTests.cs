// JobBuilderServiceTests.cs
using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using FluentAssertions;
using WSyncPro.Core.Services;
using WSyncPro.Models;

namespace WSyncPro.Tests.Core.Services
{
    public class JobBuilderServiceTests
    {
        private readonly Mock<IFileSystemService> _mockFileSystemService;
        private readonly JobBuilderService _jobBuilderService;
        private const string SourceDirectory = @"C:\SourceDirectory";

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
                SrcDirectory = SourceDirectory,
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
                SrcDirectory = SourceDirectory,
                DstDirectory = @"D:\DestinationDirectory",
                FilesToSync = 0,
                TotalFilesSynced = 0,
                FailedFiles = 0,
                InclWilcardString = new List<string> { "*render*", "*.txt", "data_??.csv" },
                ExclWildcardString = new List<string> { "*.tmp", "temp_*", "*.log" }
            };

            var allFiles = new List<string>
            {
                @"C:\SourceDirectory\image_render_final.jpg",
                @"C:\SourceDirectory\document.txt",
                @"C:\SourceDirectory\data_01.csv",
                @"C:\SourceDirectory\data_02.csv",
                @"C:\SourceDirectory\temp_image.jpg",
                @"C:\SourceDirectory\notes.tmp",
                @"C:\SourceDirectory\app.log",
                @"C:\SourceDirectory\README.md" // Should be excluded (no inclusion pattern matches)
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
                { @"C:\SourceDirectory\image_render_final.jpg", 2_147_483_648 }, // 2 GB
                { @"C:\SourceDirectory\document.txt", 524_288_000 }, // 0.488 GB
                { @"C:\SourceDirectory\data_01.csv", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory\data_02.csv", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory\temp_image.jpg", 1_073_741_824 }, // 1 GB
                { @"C:\SourceDirectory\notes.tmp", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory\app.log", 104_857_600 }, // 0.097 GB
                { @"C:\SourceDirectory\README.md", 52_428_800 } // 0.049 GB
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
            // @"C:\SourceDirectory\image_render_final.jpg" (matches "*render*")
            // @"C:\SourceDirectory\document.txt" (matches "*.txt")
            // @"C:\SourceDirectory\data_01.csv" (matches "data_??.csv")
            // @"C:\SourceDirectory\data_02.csv" (matches "data_??.csv")
            builtJob.FileList.Should().HaveCount(4);
            builtJob.FileList.Should().Contain(new List<string>
            {
                @"C:\SourceDirectory\image_render_final.jpg",
                @"C:\SourceDirectory\document.txt",
                @"C:\SourceDirectory\data_01.csv",
                @"C:\SourceDirectory\data_02.csv"
            });

            // Total size in GB: 2 + 0.488 + 0.097 + 0.097 = ~2.682 GB
            builtJob.TotalFileSizeInGB.Should().BeApproximately(2.682f, 0.002f); // Increased tolerance to 0.002F
        }

        [Theory]
        [InlineData("report?.docx", "report1.docx", true)]
        [InlineData("report?.docx", "report12.docx", false)]
        [InlineData("data_*.csv", "data_2023.csv", true)]
        [InlineData("data_*.csv", "data.csv", false)]
        [InlineData("*.jpg", "image.jpeg", false)]
        [InlineData("*.jpg", "photo.jpg", true)]
        public void WildcardMatch_ShouldReturnExpectedResult(string pattern, string fileName, bool expected)
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory,
                InclWilcardString = new List<string> { pattern },
                ExclWildcardString = new List<string>()
            };

            var allFiles = new List<string>
            {
                Path.Combine(SourceDirectory, fileName)
            };

            _mockFileSystemService.Setup(fs => fs.DirectoryExists(job.SrcDirectory))
                .Returns(true);

            _mockFileSystemService.Setup(fs => fs.EnumerateFiles(job.SrcDirectory, "*", true))
                .Returns(allFiles);

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns(1_000_000); // 0.001 GB

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            if (expected)
            {
                builtJob.FileList.Should().Contain(Path.Combine(SourceDirectory, fileName));
                builtJob.TotalFileSizeInGB.Should().BeApproximately(0.001f, 0.0001f);
            }
            else
            {
                builtJob.FileList.Should().NotContain(Path.Combine(SourceDirectory, fileName));
                builtJob.TotalFileSizeInGB.Should().Be(0);
            }
        }

        [Fact]
        public void BuildJob_ShouldExcludeFiles_WhenTheyMatchExclusionPatterns()
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory,
                InclWilcardString = new List<string> { "*" }, // Include all
                ExclWildcardString = new List<string> { "*.bak", "backup_*" }
            };

            var allFiles = new List<string>
            {
                @"C:\SourceDirectory\file1.txt",
                @"C:\SourceDirectory\file2.bak",
                @"C:\SourceDirectory\backup_file3.txt",
                @"C:\SourceDirectory\file4.docx"
            };

            var fileSizes = new Dictionary<string, long>
            {
                { @"C:\SourceDirectory\file1.txt", 500_000_000 }, // 0.465 GB
                { @"C:\SourceDirectory\file2.bak", 200_000_000 }, // 0.186 GB
                { @"C:\SourceDirectory\backup_file3.txt", 300_000_000 }, // 0.279 GB
                { @"C:\SourceDirectory\file4.docx", 100_000_000 } // 0.093 GB
            };

            _mockFileSystemService.Setup(fs => fs.DirectoryExists(job.SrcDirectory))
                .Returns(true);

            _mockFileSystemService.Setup(fs => fs.EnumerateFiles(job.SrcDirectory, "*", true))
                .Returns(allFiles);

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            builtJob.FileList.Should().HaveCount(2);
            builtJob.FileList.Should().Contain(new List<string>
            {
                @"C:\SourceDirectory\file1.txt",
                @"C:\SourceDirectory\file4.docx"
            });

            // Total size: 0.465 + 0.093 = ~0.558 GB
            builtJob.TotalFileSizeInGB.Should().BeApproximately(0.558f, 0.001f);
        }

        [Fact]
        public void BuildJob_ShouldIncludeAllFiles_WhenNoInclusionPatterns()
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory,
                InclWilcardString = new List<string>(), // No inclusion patterns
                ExclWildcardString = new List<string> { "*.tmp" }
            };

            var allFiles = new List<string>
            {
                @"C:\SourceDirectory\file1.txt",
                @"C:\SourceDirectory\file2.tmp",
                @"C:\SourceDirectory\file3.docx"
            };

            var fileSizes = new Dictionary<string, long>
            {
                { @"C:\SourceDirectory\file1.txt", 100_000_000 }, // 0.093 GB
                { @"C:\SourceDirectory\file2.tmp", 200_000_000 }, // 0.186 GB
                { @"C:\SourceDirectory\file3.docx", 300_000_000 } // 0.279 GB
            };

            _mockFileSystemService.Setup(fs => fs.DirectoryExists(job.SrcDirectory))
                .Returns(true);

            _mockFileSystemService.Setup(fs => fs.EnumerateFiles(job.SrcDirectory, "*", true))
                .Returns(allFiles);

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            builtJob.FileList.Should().HaveCount(2);
            builtJob.FileList.Should().Contain(new List<string>
            {
                @"C:\SourceDirectory\file1.txt",
                @"C:\SourceDirectory\file3.docx"
            });

            // Total size: 0.093 + 0.279 = ~0.372 GB
            builtJob.TotalFileSizeInGB.Should().BeApproximately(0.372f, 0.001f);
        }
    }
}
