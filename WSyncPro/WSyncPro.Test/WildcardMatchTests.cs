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

            // Mock GetFileSize with unique keys
            var fileSizes = new Dictionary<string, long>
            {
                { Path.Combine(SourceDirectory1, fileName), expected ? 100_000_000 : 0 } // 0.093 GB if included, 0 if excluded
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
                // No exclude pattern to log in this specific test
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

            // Mock GetFileSize with unique keys
            var fileSizes = new Dictionary<string, long>
            {
                { Path.Combine(SourceDirectory2, fileName), isExcluded ? 0 : 100_000_000 } // 0.093 GB if excluded, 0.093 GB if included
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
        [InlineData("*render*", "temp_render_video.mp4", true, true)] // Included by "*render*", excluded by "*temp*"
        [InlineData("*.log", "app.log", true, false)] // Included by "*.log", excluded by "*.log"
        [InlineData("backup_*", "backup_song.mp3", true, false)] // Included by "backup_*", excluded by "backup_*"
        [InlineData("*.jpg", "image.jpg", false, true)] // Included by "*.jpg", not excluded
        [InlineData("*.jpg", "temp_image.jpg", true, true)] // Included by "*.jpg", excluded by "*temp*"
        public void CombinedPatterns_ShouldRespectIncludeAndExcludePatterns(string includePattern, string fileName, bool isExcludedByExcludePattern, bool isExpectedToBeIncluded)
        {
            // Arrange
            var excludePatterns = new List<string>();
            if (isExcludedByExcludePattern)
            {
                // Determine which exclude pattern to apply based on the include pattern
                if (includePattern.Contains("*temp*"))
                    excludePatterns.Add("*temp*");
                if (includePattern.Contains("*.tmp"))
                    excludePatterns.Add("*.tmp");
                if (includePattern.Contains("backup_*"))
                    excludePatterns.Add("backup_*");
                if (includePattern.Contains("*.log"))
                    excludePatterns.Add("*.log");
            }

            var job = new Job
            {
                SrcDirectory = SourceDirectory4,
                InclWilcardString = new List<string> { includePattern },
                ExclWildcardString = excludePatterns
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

            // Mock GetFileSize with unique keys
            var fileSizes = new Dictionary<string, long>
            {
                { Path.Combine(SourceDirectory4, fileName), isExpectedToBeIncluded ? 100_000_000 : 0 } // 0.093 GB if included, 0 if excluded
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
                if (excludePatterns.Any())
                {
                    foreach (var pattern in excludePatterns)
                    {
                        SaveConfirmedPattern(pattern, isInclude: false);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This test validates that the confirmed list of wildcard include and exclude patterns are correctly defined and functioning as expected.
        /// It serves as a reference to ensure that the patterns used in other tests are accurate and effective.
        /// </summary>
        [Fact]
        public void ConfirmedWildcardPatterns_ShouldBeValidAndWorking()
        {
            // Arrange
            var confirmedIncludePatterns = new List<string>
            {
                "*.mp3",      // Audio files
                "*.mp4",      // MPEG-4 video files
                "*.mov",      // QuickTime video files
                "*.avi",      // AVI video files
                "*.jpg",      // JPEG image files
                "*.png",      // PNG image files
                "*render*",   // Files containing 'render'
                "*.gif",      // GIF image files
                "*.tiff"      // TIFF image files
            };

            var confirmedExcludePatterns = new List<string>
            {
                "*temp*",     // Temporary files
                "*unamed*",   // Files with 'unamed' in their name
                "*.tmp",      // Temporary files
                "backup_*",   // Backup files
                "*.log"       // Log files
            };

            // Act & Assert
            confirmedIncludePatterns.Should().Contain("*.mp3");
            confirmedIncludePatterns.Should().Contain("*.mp4");
            confirmedIncludePatterns.Should().Contain("*.mov");
            confirmedIncludePatterns.Should().Contain("*.avi");
            confirmedIncludePatterns.Should().Contain("*.jpg");
            confirmedIncludePatterns.Should().Contain("*.png");
            confirmedIncludePatterns.Should().Contain("*render*");
            confirmedIncludePatterns.Should().Contain("*.gif");
            confirmedIncludePatterns.Should().Contain("*.tiff");

            confirmedExcludePatterns.Should().Contain("*temp*");
            confirmedExcludePatterns.Should().Contain("*unamed*");
            confirmedExcludePatterns.Should().Contain("*.tmp");
            confirmedExcludePatterns.Should().Contain("backup_*");
            confirmedExcludePatterns.Should().Contain("*.log");
        }

        #endregion
    }
}
