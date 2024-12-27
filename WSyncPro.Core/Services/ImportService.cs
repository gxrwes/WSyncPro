// ImportService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WSyncPro.Models.Files;
using WSyncPro.Models.Filter;
using WSyncPro.Models.Import;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public class ImportService : IImportService
    {
        private readonly ImportPathBuilder _pathBuilder;
        private readonly ILogger<ImportService> _logger;
        private readonly ICopyService _copyService;

        public ImportService(ILogger<ImportService> logger, ICopyService copyService)
        {
            _pathBuilder = new ImportPathBuilder();
            _logger = logger;
            _copyService = copyService;
        }

        public async Task<WDirectory> GetFoundFiles(string path, FilterParams filterParams)
        {
            _logger.LogInformation("Finding files in path: {Path}", path);
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(filePath => new FileInfo(filePath))
                .Where(fileInfo => MatchesFilter(fileInfo, filterParams))
                .Select(fileInfo => new WFile
                {
                    Id = Guid.NewGuid(),
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    FileExtension = fileInfo.Extension,
                    FileSize = (int)fileInfo.Length,
                    Created = fileInfo.CreationTime,
                    LastUpdated = fileInfo.LastWriteTime
                })
                .Cast<WBaseItem>()
                .ToList();

            return new WDirectory
            {
                Id = Guid.NewGuid(),
                Name = Path.GetFileName(path),
                Path = path,
                Items = files,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            };
        }

        public async Task<string> GenerateDstPathForfile(WFile file, List<ImportPathType> pathBuilder)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (pathBuilder == null) throw new ArgumentNullException(nameof(pathBuilder));

            return await Task.FromResult(_pathBuilder.GetBuiltPath(file, pathBuilder, 0));
        }

        public async Task<List<CopyJob>> CreateCopyJobsFromDirectory(WDirectory src, string importDirectory, List<ImportPathType> pathBuilder)
        {
            _logger.LogInformation("Creating copy jobs for directory: {Directory}", src.Path);

            if (src == null || src.Items == null || !src.Items.Any())
                throw new ArgumentException("Source directory is empty or null", nameof(src));

            if (string.IsNullOrEmpty(importDirectory))
                throw new ArgumentException("Import directory cannot be null or empty", nameof(importDirectory));

            if (pathBuilder == null || !pathBuilder.Any())
                throw new ArgumentException("Path builder cannot be null or empty", nameof(pathBuilder));

            var copyJobs = new List<CopyJob>();
            int currentCounter = 0;

            foreach (var item in src.Items.OfType<WFile>())
            {
                var relativePath = _pathBuilder.GetBuiltPath(item, pathBuilder, currentCounter);
                var dstPath = Path.Combine(importDirectory, relativePath);

                copyJobs.Add(new CopyJob
                {
                    Guid = Guid.NewGuid(),
                    SrcFilePathAbsolute = item.Path,
                    DstFilePathAbsolute = dstPath,
                    Timestamp = DateTime.Now,
                    Overwrite = false,
                    Successful = false
                });

                currentCounter++;
            }

            return await Task.FromResult(copyJobs);
        }

        public async Task<bool> RunImport(string srcPath, string dstPath, FilterParams filterParams, List<ImportPathType> pathBuilder)
        {
            _logger.LogInformation("Running import from {SrcPath} to {DstPath}", srcPath, dstPath);

            if (string.IsNullOrEmpty(srcPath))
                throw new ArgumentException("Source path cannot be null or empty", nameof(srcPath));

            if (string.IsNullOrEmpty(dstPath))
                throw new ArgumentException("Destination path cannot be null or empty", nameof(dstPath));

            if (filterParams == null)
                throw new ArgumentNullException(nameof(filterParams));

            if (pathBuilder == null || !pathBuilder.Any())
                throw new ArgumentException("Path builder cannot be null or empty", nameof(pathBuilder));

            try
            {
                var foundDirectory = await GetFoundFiles(srcPath, filterParams);
                if (foundDirectory.Items == null || !foundDirectory.Items.OfType<WFile>().Any())
                {
                    _logger.LogWarning("No matching files found in source directory.");
                    return false;
                }

                var copyJobs = await CreateCopyJobsFromDirectory(foundDirectory, dstPath, pathBuilder);
                if (copyJobs == null || !copyJobs.Any())
                {
                    _logger.LogWarning("No copy jobs created.");
                    return false;
                }

                foreach (var job in copyJobs)
                {
                    await _copyService.CopyFile(job);
                }

                return copyJobs.All(job => job.Successful);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import process.");
                return false;
            }
        }

        private bool MatchesFilter(FileInfo file, FilterParams filterParams)
        {
            if (filterParams == null) return true;

            if (filterParams.Include.Any() && !filterParams.Include.Any(pattern => WildcardMatch(file.Name, pattern)))
                return false;

            if (filterParams.Exclude.Any() && filterParams.Exclude.Any(pattern => WildcardMatch(file.Name, pattern)))
                return false;

            if (filterParams.FileTypes.Any() && !filterParams.FileTypes.Contains(file.Extension))
                return false;

            if (filterParams.MaxFileSize > 0 && file.Length > filterParams.MaxFileSize)
                return false;

            if (filterParams.MinFileSize > 0 && file.Length < filterParams.MinFileSize)
                return false;

            return true;
        }

        private bool WildcardMatch(string input, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }

}
