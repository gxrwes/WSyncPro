// WildcardMatchTests.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Xunit;
using FluentAssertions;
using WSyncPro.Core.Services;
using WSyncPro.Models;

namespace WSyncPro.Tests.Core.Services
{
    public class WildcardMatchTests : BaseTest
    {
        private readonly Mock<IFileSystemService> _mockFileSystemService;
        private readonly JobBuilderService _jobBuilderService;

        // Define four varying test directories
        private const string SourceDirectory1 = @"C:\SourceDirectory1";
        private const string SourceDirectory2 = @"C:\SourceDirectory2";
        private const string SourceDirectory3 = @"D:\SourceDirectory3";
        private const string SourceDirectory4 = @"E:\SourceDirectory4";

        // Define include and exclude patterns
        private static readonly List<string> IncludePatterns = new List<string>
        {
            "*.mp3", "*.mp4", "*.mov", "*.avi", "*.jpg", "*.png", "*render*", "*.gif", "*.tiff"
        };

        private static readonly List<string> ExcludePatterns = new List<string>
        {
            "*temp*", "*unamed*", "*.tmp", "backup_*", "*.log"
        };

        public WildcardMatchTests()
        {
            _mockFileSystemService = new Mock<IFileSystemService>();
            _jobBuilderService = new JobBuilderService(_mockFileSystemService.Object);
        }

        #region Include Pattern Tests

        /// <summary>
        /// Tests that files matching inclusion patterns are correctly included.
        /// </summary>
        [Theory]
        [InlineData("*.mp3", "song.mp3", true)]
        [InlineData("*.mp3", "song.wav", false)]
        [InlineData("*.mp4", "video.mp4", true)]
        [InlineData("*.mp4", "video.mov", false)]
        [InlineData("*.mov", "movie.mov", true)]
        [InlineData("*.mov", "movie.mp4", false)]
        [InlineData("*.avi", "clip.avi", true)]
        [InlineData("*.avi", "clip.mp4", false)]
        [InlineData("*.jpg", "photo.jpg", true)]
        [InlineData("*.jpg", "photo.png", false)]
        [InlineData("*.png", "image.png", true)]
        [InlineData("*.png", "image.jpg", false)]
        [InlineData("*render*", "video_render_final.mp4", true)]
        [InlineData("*render*", "video_final.mp4", false)]
        [InlineData("*mp3", "audio.mp3", true)] // Additional include pattern
        [InlineData("*mp3", "audio.mp3a", false)] // Non-matching include pattern
        public void IncludePatterns_ShouldIncludeMatchingFiles(string pattern, string fileName, bool expected)
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory1,
                InclWilcardString = new List<string> { pattern },
                ExclWildcardString = new List<string>() // No exclude patterns
            };

            var allFiles = new List<string>
            {
                Path.Combine(SourceDirectory1, fileName)
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
                { Path.Combine(SourceDirectory1, "song.mp3"), 5_000_000 },    // 0.0047 GB
                { Path.Combine(SourceDirectory1, "song.wav"), 5_000_000 },    // 0.0047 GB
                { Path.Combine(SourceDirectory1, "video.mp4"), 10_000_000 },  // 0.0093 GB
                { Path.Combine(SourceDirectory1, "video.mov"), 10_000_000 },  // 0.0093 GB
                { Path.Combine(SourceDirectory1, "movie.mov"), 10_000_000 },  // 0.0093 GB
                { Path.Combine(SourceDirectory1, "movie.mp4"), 10_000_000 },  // 0.0093 GB
                { Path.Combine(SourceDirectory1, "clip.avi"), 8_000_000 },    // 0.00745 GB
                { Path.Combine(SourceDirectory1, "clip.mp4"), 8_000_000 },    // 0.00745 GB
                { Path.Combine(SourceDirectory1, "photo.jpg"), 3_000_000 },   // 0.0028 GB
                { Path.Combine(SourceDirectory1, "photo.png"), 3_000_000 },   // 0.0028 GB
                { Path.Combine(SourceDirectory1, "image.png"), 3_000_000 },   // 0.0028 GB
                { Path.Combine(SourceDirectory1, "image.jpg"), 3_000_000 },   // 0.0028 GB
                { Path.Combine(SourceDirectory1, "video_render_final.mp4"), 12_000_000 }, // 0.0112 GB
                { Path.Combine(SourceDirectory1, "audio.mp3"), 5_000_000 },   // 0.0047 GB
                { Path.Combine(SourceDirectory1, "audio.mp3a"), 0 }           // 0 GB
            };

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            var expectedFilePath = Path.Combine(SourceDirectory1, fileName);
            if (expected)
            {
                builtJob.FileList.Should().Contain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().BeApproximately(fileSizes[expectedFilePath] / (1024f * 1024f * 1024f), 0.0001f);
                // Log confirmed include pattern
                SaveConfirmedPattern(pattern, isInclude: true);
            }
            else
            {
                builtJob.FileList.Should().NotContain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().Be(0);
                // Log confirmed exclude pattern (none in this test)
            }
        }

        #endregion

        #region Exclude Pattern Tests

        /// <summary>
        /// Tests that files matching exclusion patterns are correctly excluded.
        /// </summary>
        [Theory]
        [InlineData("*.tmp", "file1.tmp", true)]
        [InlineData("*.tmp", "file1.txt", false)]
        [InlineData("*temp*", "temp_video.mp4", true)]
        [InlineData("*temp*", "video.mp4", false)]
        [InlineData("*unamed*", "video_unamed_clip.mp4", true)]
        [InlineData("*unamed*", "video_clip.mp4", false)]
        [InlineData("backup_*", "backup_video.mp4", true)]
        [InlineData("backup_*", "video.mp4", false)]
        [InlineData("*.log", "error.log", true)]
        [InlineData("*.log", "error.txt", false)]
        public void ExcludePatterns_ShouldExcludeMatchingFiles(string pattern, string fileName, bool isExcluded)
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory2,
                InclWilcardString = new List<string> { "*" }, // Include all files
                ExclWildcardString = new List<string> { pattern }
            };

            var allFiles = new List<string>
            {
                Path.Combine(SourceDirectory2, fileName)
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
                { Path.Combine(SourceDirectory2, "file1.tmp"), 200_000_000 },   // 0.186 GB
                { Path.Combine(SourceDirectory2, "file1.txt"), 100_000_000 },   // 0.093 GB
                { Path.Combine(SourceDirectory2, "temp_video.mp4"), 500_000_000 }, // 0.465 GB
                { Path.Combine(SourceDirectory2, "video.mp4"), 500_000_000 },     // 0.465 GB
                { Path.Combine(SourceDirectory2, "video_unamed_clip.mp4"), 300_000_000 }, // 0.279 GB
                { Path.Combine(SourceDirectory2, "video_clip.mp4"), 300_000_000 },      // 0.279 GB
                { Path.Combine(SourceDirectory2, "backup_video.mp4"), 400_000_000 },    // 0.372 GB
                { Path.Combine(SourceDirectory2, "video.mp4"), 400_000_000 },           // 0.372 GB
                { Path.Combine(SourceDirectory2, "error.log"), 150_000_000 },          // 0.139 GB
                { Path.Combine(SourceDirectory2, "error.txt"), 100_000_000 }           // 0.093 GB
            };

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            var expectedFilePath = Path.Combine(SourceDirectory2, fileName);
            if (isExcluded)
            {
                builtJob.FileList.Should().NotContain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().Be(0);
                // Log confirmed exclude pattern
                SaveConfirmedPattern(pattern, isInclude: false);
            }
            else
            {
                builtJob.FileList.Should().Contain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().BeApproximately(fileSizes[expectedFilePath] / (1024f * 1024f * 1024f), 0.0001f);
                // Log confirmed include pattern (all included)
                SaveConfirmedPattern("*", isInclude: true);
            }
        }

        #endregion

        #region Combined Include and Exclude Pattern Tests

        /// <summary>
        /// Tests that files are correctly included or excluded based on combined inclusion and exclusion patterns.
        /// Exclusion patterns should take precedence over inclusion patterns.
        /// </summary>
        [Theory]
        [InlineData("*.mp3", "song.mp3", false, true)] // Included by pattern, not excluded
        [InlineData("*.mp3", "song.tmp", true, false)] // Included by pattern, excluded by "*.tmp"
        [InlineData("*render*", "video_render_final.mp4", false, true)] // Included by "*render*", not excluded
        [InlineData("*render*", "temp_render_video.mp4", true, false)] // Included by "*render*", excluded by "*temp*"
        [InlineData("*.log", "app.log", true, false)] // Included by "*.log", excluded by "*.log"
        [InlineData("backup_*", "backup_song.mp3", true, false)] // Included by "backup_*", excluded by "backup_*"
        [InlineData("*.jpg", "image.jpg", false, true)] // Included by "*.jpg", not excluded
        [InlineData("*.jpg", "temp_image.jpg", true, false)] // Included by "*.jpg", excluded by "*temp*"
        public void CombinedPatterns_ShouldRespectIncludeAndExcludePatterns(string includePattern, string fileName, bool isExcludedByExcludePattern, bool isExpectedToBeIncluded)
        {
            // Arrange
            var job = new Job
            {
                SrcDirectory = SourceDirectory4,
                InclWilcardString = new List<string> { includePattern },
                ExclWildcardString = new List<string> { isExcludedByExcludePattern ? "*.tmp" : string.Empty, isExcludedByExcludePattern ? "*temp*" : string.Empty } // Conditional exclude patterns
            };

            var allFiles = new List<string>
            {
                Path.Combine(SourceDirectory4, fileName)
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
                { Path.Combine(SourceDirectory4, "song.mp3"), 5_000_000 },             // 0.0047 GB
                { Path.Combine(SourceDirectory4, "song.tmp"), 200_000_000 },           // 0.186 GB
                { Path.Combine(SourceDirectory4, "video_render_final.mp4"), 10_000_000 },// 0.0093 GB
                { Path.Combine(SourceDirectory4, "temp_render_video.mp4"), 10_000_000 }, // 0.0093 GB
                { Path.Combine(SourceDirectory4, "app.log"), 150_000_000 },             // 0.139 GB
                { Path.Combine(SourceDirectory4, "backup_song.mp3"), 5_000_000 },        // 0.0047 GB
                { Path.Combine(SourceDirectory4, "image.jpg"), 3_000_000 },            // 0.0028 GB
                { Path.Combine(SourceDirectory4, "temp_image.jpg"), 3_000_000 }        // 0.0028 GB
            };

            _mockFileSystemService.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
                .Returns<string>(filePath => fileSizes.ContainsKey(filePath) ? fileSizes[filePath] : 0);

            // Act
            BuiltJob builtJob = _jobBuilderService.BuildJob(job);

            // Assert
            var expectedFilePath = Path.Combine(SourceDirectory4, fileName);
            if (isExpectedToBeIncluded)
            {
                builtJob.FileList.Should().Contain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().BeApproximately(fileSizes[expectedFilePath] / (1024f * 1024f * 1024f), 0.0001f);
                // Log confirmed include pattern
                SaveConfirmedPattern(includePattern, isInclude: true);
            }
            else
            {
                builtJob.FileList.Should().NotContain(expectedFilePath);
                builtJob.TotalFileSizeInGB.Should().Be(0);
                // Log confirmed exclude pattern
                SaveConfirmedPattern(isExcludedByExcludePattern ? "*.tmp" : "*temp*", isInclude: false);
            }
        }

        #endregion
    }
}
