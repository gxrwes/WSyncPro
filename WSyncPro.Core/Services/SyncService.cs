using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Filter;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IAppCache _cache;
        private readonly ICopyService _copyService;
        private readonly IFileVersioning _fileVersioning;
        private readonly ILogger<SyncService> _logger;

        public SyncService(IAppCache cache, ICopyService copyService, IFileVersioning fileVersioning, ILogger<SyncService> logger)
        {
            _cache = cache;
            _copyService = copyService;
            _fileVersioning = fileVersioning;
            _logger = logger;
        }

        public async Task<List<CopyJob>> CreateCpJobsForSyncJob(SyncJob job)
        {
            try
            {
                if (job == null)
                    throw new ArgumentNullException(nameof(job));

                _logger.LogInformation("Starting sync job with ID {JobId}", job.Id);

                // 1. Scan source directory
                var sourceDirectory = await ScanDirectoryAsync(job.SrcDirectory);
                _cache.AddDirectory(sourceDirectory);

                // 2. Filter files
                var filteredFiles = ApplyFilters(sourceDirectory, job.FilterParams);

                // 3. Scan target directory
                var targetDirectory = await ScanDirectoryAsync(job.DstDirectory);
                _cache.AddDirectory(targetDirectory);

                // 4. Generate copy jobs
                var copyJobs = GenerateCopyJobs(filteredFiles, targetDirectory, job);
                foreach (var jobItem in copyJobs)
                {
                    await _cache.AddCopyJob(jobItem);
                }

                return copyJobs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating copy jobs for sync job {JobId}", job?.Id);
                throw;
            }
        }

        public async Task<List<CopyJob>> CreateCpJobsForSyncJobs(List<SyncJob> jobs)
        {
            var allCopyJobs = new List<CopyJob>();

            foreach (var job in jobs)
            {
                var copyJobs = await CreateCpJobsForSyncJob(job);
                allCopyJobs.AddRange(copyJobs);
            }

            return allCopyJobs;
        }

        public async Task<List<bool>> ExecuteAndVerifyJobs(List<CopyJob> jobs)
        {
            var results = new List<bool>();

            foreach (var job in jobs)
            {
                try
                {
                    await _copyService.CopyFile(job);
                    _logger.LogInformation("Copy job executed successfully for file {FileName}", job.SrcFilePathAbsolute);
                    results.Add(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing copy job for file {FileName}", job.SrcFilePathAbsolute);
                    results.Add(false);
                }
            }

            return results;
        }

        public async Task<WDirectory> ScanDirectoryAsync(string path)
        {
            var directory = new WDirectory { Path = path, Name = Path.GetFileName(path) };

            try
            {
                var files = Directory.GetFiles(path);
                var subdirectories = Directory.GetDirectories(path);

                directory.Items = new List<WBaseItem>();

                foreach (var filePath in files)
                {
                    var fileInfo = new FileInfo(filePath);
                    var wFile = new WFile
                    {
                        Path = filePath,
                        Name = fileInfo.Name,
                        FileSize = (int)fileInfo.Length,
                        LastUpdated = fileInfo.LastWriteTime,
                        Created = fileInfo.CreationTime,
                        FileExtension = fileInfo.Extension
                    };

                    directory.Items.Add(wFile);
                }

                foreach (var subdirectoryPath in subdirectories)
                {
                    var subdirectory = await ScanDirectoryAsync(subdirectoryPath);
                    directory.Items.Add(subdirectory);
                }

                return directory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning directory {DirectoryPath}", path);
                throw;
            }
        }

        private List<WFile> ApplyFilters(WDirectory directory, FilterParams filters)
        {
            var filteredFiles = new List<WFile>();

            void TraverseAndFilter(WDirectory currentDirectory)
            {
                // Process files in the current directory
                filteredFiles.AddRange(currentDirectory.Items.OfType<WFile>()
                    .Where(file =>
                        (filters.Include.Count == 0 || filters.Include.Any(include => WildCardFound(file.Name, include))) &&
                        !filters.Exclude.Any(exclude => WildCardFound(file.Name, exclude))));

                // Recursively process subdirectories
                foreach (var subDirectory in currentDirectory.Items.OfType<WDirectory>())
                {
                    TraverseAndFilter(subDirectory);
                }
            }

            TraverseAndFilter(directory);
            return filteredFiles;
        }

        private bool WildCardFound(string input, string wildcard)
        {
            if (string.IsNullOrEmpty(wildcard))
                return false;

            // Convert wildcard to a regular expression
            string pattern = "^" + System.Text.RegularExpressions.Regex.Escape(wildcard)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            // Match the input against the pattern
            return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        }



        private List<CopyJob> GenerateCopyJobs(List<WFile> sourceFiles, WDirectory targetDirectory, SyncJob job)
        {
            var copyJobs = new List<CopyJob>();

            foreach (var sourceFile in sourceFiles)
            {
                var targetPath = job.KeepDirectories
                    ? sourceFile.Path.Replace(job.SrcDirectory, targetDirectory.Path)
                    : Path.Combine(targetDirectory.Path, sourceFile.Name);

                var existingFile = targetDirectory.Items
                    .OfType<WFile>()
                    .FirstOrDefault(f => f.Path == targetPath);

                if (existingFile == null ||
                    existingFile.LastUpdated != sourceFile.LastUpdated ||
                    existingFile.FileSize != sourceFile.FileSize)
                {
                    copyJobs.Add(new CopyJob
                    {
                        SrcFilePathAbsolute = sourceFile.Path,
                        DstFilePathAbsolute = targetPath
                    });
                }
            }

            return copyJobs;
        }
    }

}
