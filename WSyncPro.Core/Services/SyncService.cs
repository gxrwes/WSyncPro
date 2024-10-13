using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.Enum;
using WSyncPro.Util.Files;
using WSyncPro.Util.Services;

namespace WSyncPro.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IFileLoader _fileLoader;
        private readonly IDirectoryScannerService _directoryScannerService;
        private readonly IFileCopyMoveService _fileCopyMoveService;
        private List<SyncJob> _jobs = new List<SyncJob>();

        public SyncService(IFileLoader fileLoader, IDirectoryScannerService directoryScannerService, IFileCopyMoveService fileCopyMoveService)
        {
            _fileLoader = fileLoader;
            _directoryScannerService = directoryScannerService;
            _fileCopyMoveService = fileCopyMoveService;
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
                existingJob.Selected = job.Selected;
            }
            else
            {
                _jobs.Add(job);
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
                    existingJob.Selected = job.Selected;
                }
                else
                {
                    _jobs.Add(job);
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<SyncJob>> GetAllJobs()
        {
            return Task.FromResult(_jobs);
        }

        public Task<SyncJob> GetJobById(int id)
        {
            var job = _jobs.FirstOrDefault(j => j.Id.GetHashCode() == id);
            return Task.FromResult(job);
        }

        public async Task LoadJoblistFromFile(string joblistFilePath)
        {
            var loadedJobs = await _fileLoader.LoadFileAndParseAsync<List<SyncJob>>(joblistFilePath);
            _jobs = loadedJobs ?? new List<SyncJob>();
        }

        public async Task SaveJoblistToFile(string joblistFilePath)
        {
            await _fileLoader.SaveToFileAsObjectAsync(joblistFilePath, _jobs);
        }

        public async Task<(Guid jobId, int filesProcessed, string message)> RunAllEnabledJobs()
        {
            int totalProcessedFiles = 0;
            Guid lastProcessedJobId = Guid.Empty;

            foreach (var job in _jobs.Where(j => j.Status == Status.Running))
            {
                var objectsToSync = await _directoryScannerService.ScanAsync(job);

                foreach (var wObject in objectsToSync)
                {
                    var sourcePath = wObject.FullPath;
                    var destinationPath = System.IO.Path.Combine(job.DstDirectory, wObject.Name);

                    if (wObject is WFile wFile && job.Status == Status.Running)
                    {
                        await _fileCopyMoveService.CopyFileAsync(sourcePath, destinationPath);
                        totalProcessedFiles++;
                    }
                }

                lastProcessedJobId = job.Id;
            }

            return (lastProcessedJobId, totalProcessedFiles, "Jobs Completed");
        }
    }
}
