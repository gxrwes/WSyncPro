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
using WSyncPro.Core.Services;
using WSyncPro.Models.Reporting;
using System.Diagnostics; // Ensure the namespace includes ProgressChangedEventArgs

namespace WSyncPro.Core.Services
{
    public class SyncService : ISyncService
    {
        private const string JOB_LIST_FILE = "./joblist.json";
        private readonly IFileLoader _fileLoader;
        private readonly IDirectoryScannerService _directoryScannerService;
        private readonly IFileCopyMoveService _fileCopyMoveService;
        private readonly ILogger<SyncService> _logger;
        private List<SyncJob> _jobs = new List<SyncJob>();

        public SyncService(
            IFileLoader fileLoader,
            IDirectoryScannerService directoryScannerService,
            IFileCopyMoveService fileCopyMoveService,
            ILogger<SyncService> logger)
        {
            _fileLoader = fileLoader;
            _directoryScannerService = directoryScannerService;
            _fileCopyMoveService = fileCopyMoveService;
            _logger = logger;
        }

        // Implementing the event from the interface
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

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

        public async Task LoadJoblistFromFile(string joblistFilePath = JOB_LIST_FILE)
        {
            //ignore custome filepath for now till we have env handling
            joblistFilePath = JOB_LIST_FILE; _logger.LogInformation($"Loading jobs from file: {joblistFilePath}");
            var loadedJobs = await _fileLoader.LoadFileAndParseAsync<List<SyncJob>>(joblistFilePath);
            _jobs = loadedJobs ?? new List<SyncJob>();
            _logger.LogInformation($"Loaded {_jobs.Count} jobs from file");
        }

        public async Task SaveJoblistToFile(string joblistFilePath)
        {
            //ignore custome filepath for now till we have env handling
            joblistFilePath = JOB_LIST_FILE;
            _logger.LogInformation($"Saving jobs to file: {joblistFilePath}");
            await _fileLoader.SaveToFileAsObjectAsync(joblistFilePath, _jobs);
            _logger.LogInformation($"Saved {_jobs.Count} jobs to file");
        }

        public async Task<SyncSummary> RunAllEnabledJobs()
        {
            var syncSummary = new SyncSummary();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var enabledJobs = _jobs.Where(j => j.Enabled).ToList();
            syncSummary.TotalJobs = enabledJobs.Count;
            int currentJobIndex = 0;

            foreach (var job in enabledJobs)
            {
                currentJobIndex++;
                _logger.LogInformation($"Running job: {job.Name}");

                var jobSummary = new JobSummary
                {
                    JobId = job.Id,
                    JobName = job.Name,
                };
                var jobStopwatch = Stopwatch.StartNew();

                // Notify progress at the start of the job
                ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
                {
                    JobId = job.Id,
                    JobName = job.Name,
                    TotalJobs = syncSummary.TotalJobs,
                    CurrentJobIndex = currentJobIndex,
                    Message = $"Starting job {job.Name}"
                });

                var objectsToSync = await _directoryScannerService.ScanAsync(job);
                jobSummary.TotalFiles = objectsToSync.Count();
                int processedFiles = 0;
                int failedFiles = 0;

                foreach (var wObject in objectsToSync)
                {
                    var sourcePath = wObject.FullPath;
                    var destinationPath = System.IO.Path.Combine(job.DstDirectory, wObject.Name);

                    if (wObject is WFile wFile)
                    {
                        try
                        {
                            await _fileCopyMoveService.CopyFileAsync(sourcePath, destinationPath);
                            processedFiles++;
                            syncSummary.TotalFilesProcessed++;
                            _logger.LogDebug($"Copied file: {sourcePath} to {destinationPath}");
                        }
                        catch (Exception ex)
                        {
                            failedFiles++;
                            syncSummary.TotalFilesFailed++;
                            _logger.LogError($"Failed to copy file: {sourcePath} to {destinationPath}. Error: {ex.Message}");
                        }

                        // Notify progress for each file copied
                        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
                        {
                            JobId = job.Id,
                            JobName = job.Name,
                            TotalJobs = syncSummary.TotalJobs,
                            CurrentJobIndex = currentJobIndex,
                            TotalFiles = jobSummary.TotalFiles,
                            ProcessedFiles = processedFiles,
                            Message = $"Processed file {wFile.Name}"
                        });
                    }
                }

                jobStopwatch.Stop();
                jobSummary.FilesProcessed = processedFiles;
                jobSummary.FilesFailed = failedFiles;
                jobSummary.JobTime = jobStopwatch.Elapsed;
                syncSummary.JobSummaries.Add(jobSummary);

                // Notify progress at the end of the job
                ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
                {
                    JobId = job.Id,
                    JobName = job.Name,
                    TotalJobs = syncSummary.TotalJobs,
                    CurrentJobIndex = currentJobIndex,
                    TotalFiles = jobSummary.TotalFiles,
                    ProcessedFiles = processedFiles,
                    Message = $"Completed job {job.Name}"
                });
            }

            stopwatch.Stop();
            syncSummary.TotalJobsCompleted = syncSummary.JobSummaries.Count;
            syncSummary.TotalTime = stopwatch.Elapsed;

            _logger.LogInformation($"Completed running jobs. Total files processed: {syncSummary.TotalFilesProcessed}");

            // Notify progress at the end of all jobs
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
            {
                Message = "All jobs completed",
                TotalJobs = syncSummary.TotalJobs,
                CurrentJobIndex = syncSummary.TotalJobsCompleted,
            });
            //await SaveJoblistToFile(JOB_LIST_FILE);
            return syncSummary;
        }
    }
}
