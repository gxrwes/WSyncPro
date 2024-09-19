using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSyncPro.Core.Managers;
using WSyncPro.Models;

namespace WSyncPro.Core.Services
{
    public class SyncService
    {
        private readonly JobBuilderService _jobBuilderService;
        private readonly FileInteractionService _fileInteractionService;
        private readonly AppStateManager _appStateManager;

        public SyncService(JobBuilderService jobBuilderService, FileInteractionService fileInteractionService)
        {
            _jobBuilderService = jobBuilderService ?? throw new ArgumentNullException(nameof(jobBuilderService));
            _fileInteractionService = fileInteractionService ?? throw new ArgumentNullException(nameof(fileInteractionService));
            _appStateManager = AppStateManager.Instance;
        }

        /// <summary>
        /// Runs the synchronization for all provided jobs.
        /// </summary>
        /// <param name="jobs">List of jobs to sync</param>
        public async Task RunSyncAsync(List<Job> jobs)
        {
            foreach (var job in jobs)
            {
                if (!job.IsEnabled)
                {
                    _appStateManager.LogMessage($"Skipping job {job.Name} (ID: {job.Id}) as it is disabled.");
                    continue;
                }

                // Build the job using JobBuilderService
                BuiltJob builtJob = _jobBuilderService.BuildJob(job);
                if (builtJob.FileList.Count == 0)
                {
                    _appStateManager.LogMessage($"No files to sync for job {job.Name} (ID: {job.Id}).");
                    continue;
                }

                try
                {
                    _appStateManager.LogMessage($"Starting sync for job {job.Name} (ID: {job.Id}) with {builtJob.FileList.Count} files.");

                    // Update the app state manager
                    await _appStateManager.UpdateJobAsync(builtJob);

                    // Perform file sync
                    await _fileInteractionService.CopyFilesAsync(builtJob.FileList, builtJob.DstDirectory, job: builtJob);

                    _appStateManager.LogMessage($"Sync completed for job {job.Name} (ID: {job.Id}).");
                }
                catch (Exception ex)
                {
                    _appStateManager.LogMessage($"Error occurred during sync for job {job.Name} (ID: {job.Id}): {ex.Message}");
                }
            }
        }
    }
}
