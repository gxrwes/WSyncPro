using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.Enum;
using WSyncPro.Util.Files;
using WSyncPro.Util.Services;
using Microsoft.Extensions.Logging;

namespace WSyncPro.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IFileLoader _fileLoader;
        private readonly IDirectoryScannerService _directoryScannerService;
        private readonly IFileCopyMoveService _fileCopyMoveService;
        private readonly ILogger<SyncService> _logger;
        private List<SyncJob> _jobs = new List<SyncJob>();

        public SyncService(IFileLoader fileLoader, IDirectoryScannerService directoryScannerService, IFileCopyMoveService fileCopyMoveService, ILogger<SyncService> logger)
        {
            _fileLoader = fileLoader;
            _directoryScannerService = directoryScannerService;
            _fileCopyMoveService = fileCopyMoveService;
            _logger = logger;
        }

        public Task AddJob(SyncJob job)
        {
            if (_jobs.Any(j => j.Id == job.Id))
            {
                var existingJob = _jobs.First(j => j.Id == job.Id);
                existingJob.Name = job.Name;
                existingJob.Description = job.Description;
                existingJob.Status = job.Status;
                existingJob.SrcDirectory = job.SrcDirectory;
                existingJob.DstDirectory = job.DstDirectory;
                existingJob.FilterInclude = job.FilterInclude;
                existingJob.FilterExclude = job.FilterExclude;
                existingJob.Enabled = job.Enabled;

                _logger.LogInformation($"Updated existing job: {job.Name}");
            }
            else
            {
                _jobs.Add(job);
                _logger.LogInformation($"Added new job: {job.Name}");
            }
            return Task.CompletedTask;
        }

        public Task AddJob(List<SyncJob> jobs)
        {
            foreach (var job in jobs)
            {
                if (_jobs.Any(j => j.Id == job.Id))
                {
                    var existingJob = _jobs.First(j => j.Id == job.Id);
                    existingJob.Name = job.Name;
                    existingJob.Description = job.Description;
                    existingJob.Status = job.Status;
                    existingJob.SrcDirectory = job.SrcDirectory;
                    existingJob.DstDirectory = job.DstDirectory;
                    existingJob.FilterInclude = job.FilterInclude;
                    existingJob.FilterExclude = job.FilterExclude;
                    existingJob.Enabled = job.Enabled;

                    _logger.LogInformation($"Updated existing job: {job.Name}");
                }
                else
                {
                    _jobs.Add(job);
                    _logger.LogInformation($"Added new job: {job.Name}");
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<SyncJob>> GetAllJobs()
        {
            _logger.LogDebug("Retrieved all jobs");
            return Task.FromResult(_jobs);
        }

        public Task<SyncJob> GetJobById(Guid id)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == id);
            _logger.LogDebug($"Retrieved job by Id: {id}");
            return Task.FromResult(job);
        }

        public async Task LoadJoblistFromFile(string joblistFilePath)
        {
            _logger.LogInformation($"Loading jobs from file: {joblistFilePath}");
            var loadedJobs = await _fileLoader.LoadFileAndParseAsync<List<SyncJob>>(joblistFilePath);
            _jobs = loadedJobs ?? new List<SyncJob>();
            _logger.LogInformation($"Loaded {_jobs.Count} jobs from file");
        }

        public async Task SaveJoblistToFile(string joblistFilePath)
        {
            _logger.LogInformation($"Saving jobs to file: {joblistFilePath}");
            await _fileLoader.SaveToFileAsObjectAsync(joblistFilePath, _jobs);
            _logger.LogInformation($"Saved {_jobs.Count} jobs to file");
        }

        public async Task<(Guid jobId, int filesProcessed, string message)> RunAllEnabledJobs()
        {
            int totalProcessedFiles = 0;
            Guid lastProcessedJobId = Guid.Empty;

            foreach (var job in _jobs.Where(j => j.Enabled))
            {
                _logger.LogInformation($"Running job: {job.Name}");
                var objectsToSync = await _directoryScannerService.ScanAsync(job);

                foreach (var wObject in objectsToSync)
                {
                    var sourcePath = wObject.FullPath;
                    var destinationPath = System.IO.Path.Combine(job.DstDirectory, wObject.Name);

                    if (wObject is WFile wFile && job.Status == Status.Running)
                    {
                        await _fileCopyMoveService.CopyFileAsync(sourcePath, destinationPath);
                        totalProcessedFiles++;
                        _logger.LogDebug($"Copied file: {sourcePath} to {destinationPath}");
                    }
                }

                lastProcessedJobId = job.Id;
            }

            _logger.LogInformation($"Completed running jobs. Total files processed: {totalProcessedFiles}");
            return (lastProcessedJobId, totalProcessedFiles, "Jobs Completed");
        }
    }
}
