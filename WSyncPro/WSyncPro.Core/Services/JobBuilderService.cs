// JobBuilderService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WSyncPro.Models;

namespace WSyncPro.Core.Services
{
    public class JobBuilderService
    {
        private readonly IFileSystemService _fileSystemService;

        public JobBuilderService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        }

        /// <summary>
        /// Builds a BuiltJob from the provided Job by scanning the source directory for matching files.
        /// </summary>
        /// <param name="job">The Job to build a BuiltJob from.</param>
        /// <returns>A BuiltJob containing the list of matched files and the total file size in GB.</returns>
        public BuiltJob BuildJob(Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrWhiteSpace(job.SrcDirectory))
                throw new ArgumentException("Source directory is not specified.", nameof(job.SrcDirectory));

            if (!_fileSystemService.DirectoryExists(job.SrcDirectory))
                throw new DirectoryNotFoundException($"Source directory not found: {job.SrcDirectory}");

            // Get all files from the source directory and subdirectories
            IEnumerable<string> allFiles = _fileSystemService.EnumerateFiles(job.SrcDirectory, "*", true);

            // Filter files based on inclusion and exclusion patterns
            var matchedFiles = allFiles.Where(file => IsMatch(file, job.InclWilcardString, job.ExclWildcardString)).ToList();

            // Calculate total file size in GB
            double totalSizeBytes = matchedFiles.Sum(file => _fileSystemService.GetFileSize(file));
            float totalSizeGB = (float)(totalSizeBytes / (1024 * 1024 * 1024));

            // Create and return the BuiltJob
            return new BuiltJob
            {
                Id = job.Id,
                Name = job.Name,
                Description = job.Description,
                IsEnabled = job.IsEnabled,
                SrcDirectory = job.SrcDirectory,
                DstDirectory = job.DstDirectory,
                FilesToSync = job.FilesToSync,
                TotalFilesSynced = job.TotalFilesSynced,
                FailedFiles = job.FailedFiles,
                InclWilcardString = new List<string>(job.InclWilcardString),
                ExclWildcardString = new List<string>(job.ExclWildcardString),
                FileList = matchedFiles,
                TotalFileSizeInGB = totalSizeGB
            };
        }

        /// <summary>
        /// Determines if a file matches the inclusion patterns and does not match the exclusion patterns.
        /// </summary>
        /// <param name="filePath">The full path of the file.</param>
        /// <param name="includePatterns">List of inclusion wildcard patterns.</param>
        /// <param name="excludePatterns">List of exclusion wildcard patterns.</param>
        /// <returns>True if the file should be included; otherwise, false.</returns>
        private bool IsMatch(string filePath, List<string> includePatterns, List<string> excludePatterns)
        {
            string fileName = Path.GetFileName(filePath);

            bool isIncluded = includePatterns == null || includePatterns.Count == 0
                ? true // If no include patterns, include all
                : includePatterns.Any(pattern => WildcardMatch(fileName, pattern));

            bool isExcluded = excludePatterns != null && excludePatterns.Any(pattern => WildcardMatch(fileName, pattern));

            return isIncluded && !isExcluded;
        }

        /// <summary>
        /// Checks if the input string matches the wildcard pattern.
        /// Supports '*' and '?' wildcards.
        /// </summary>
        /// <param name="input">The input string to match.</param>
        /// <param name="pattern">The wildcard pattern.</param>
        /// <returns>True if the input matches the pattern; otherwise, false.</returns>
        private bool WildcardMatch(string input, string pattern)
        {
            // Escape regex special characters except for '*' and '?'
            string regexPattern = "^" + Regex.Escape(pattern)
                                        .Replace("\\*", ".*")
                                        .Replace("\\?", ".") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
