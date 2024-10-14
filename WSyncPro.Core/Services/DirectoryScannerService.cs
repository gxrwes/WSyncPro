using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.Enum;

namespace WSyncPro.Core.Services
{
    public class DirectoryScannerService : IDirectoryScannerService
    {
        public async Task<List<WObject>> ScanAsync(SyncJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrEmpty(job.SrcDirectory) || !Directory.Exists(job.SrcDirectory))
                throw new DirectoryNotFoundException($"Source directory '{job.SrcDirectory}' does not exist.");

            var wObjects = new List<WObject>();

            await ScanDirectoryAsync(job.SrcDirectory, job, wObjects);

            return wObjects;
        }

        private async Task ScanDirectoryAsync(string directoryPath, SyncJob job, List<WObject> wObjects)
        {
            // Get all files in the directory and filter them according to the SyncJob
            var files = Directory.EnumerateFiles(directoryPath)
                .Where(file => ShouldIncludeFile(file, job.FilterInclude, job.FilterExclude));

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var wFile = new WFile
                {
                    Id = Guid.NewGuid(),
                    Name = fileInfo.Name,
                    Description = $"File scanned from {fileInfo.FullName}",
                    Type = "File",
                    FullPath = fileInfo.FullName,
                    Filesize = fileInfo.Length,
                    Filetype = fileInfo.Extension,
                    FilesizeMultiplyer = FileSizeMultiplyer.Byte,
                    WFileMetadata = new WFileMetadata
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            { "CreationTime", fileInfo.CreationTime.ToString() },
                            { "LastAccessTime", fileInfo.LastAccessTime.ToString() }
                        }
                    }
                };

                wObjects.Add(wFile);
            }

            // Recursively scan subdirectories
            var subDirectories = Directory.EnumerateDirectories(directoryPath)
                .Where(dir => !ShouldExcludeDirectory(dir, job.FilterExclude));

            foreach (var subDirectory in subDirectories)
            {
                await ScanDirectoryAsync(subDirectory, job, wObjects);
            }
        }

        private bool ShouldIncludeFile(string filePath, List<string> includeFilters, List<string> excludeFilters)
        {
            // Include files that match any pattern in the include filters
            var include = includeFilters == null || includeFilters.Count == 0 || includeFilters.Any(pattern => IsPatternMatch(filePath, pattern));

            // Exclude files that match any pattern in the exclude filters
            var exclude = excludeFilters != null && excludeFilters.Any(pattern => IsPatternMatch(filePath, pattern));

            return include && !exclude;
        }

        private bool ShouldExcludeDirectory(string directoryPath, List<string> excludeFilters)
        {
            // Exclude directories that match any pattern in the exclude filters
            return excludeFilters != null && excludeFilters.Any(pattern => IsPatternMatch(directoryPath, pattern));
        }

        private bool IsPatternMatch(string path, string pattern)
        {
            // Extract the filename from the full path
            var filename = Path.GetFileName(path);

            // Convert wildcard pattern to regex pattern
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

            // Perform the regex match on the filename only
            return Regex.IsMatch(filename, regexPattern, RegexOptions.IgnoreCase);
        }

    }
}
