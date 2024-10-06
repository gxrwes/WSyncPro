using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Core.Managers;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.State;
using WSyncPro.Util.Services;

namespace WSyncPro.Core.Services
{
    public class SyncService
    {
        private readonly IDirectoryScannerService _scannerService;
        private readonly IFileCopyMoveService _fileCopyMoveService;
        private readonly StateManager _stateManager;

        public SyncService(IDirectoryScannerService scannerService, IFileCopyMoveService fileCopyMoveService, StateManager stateManager)
        {
            _scannerService = scannerService;
            _fileCopyMoveService = fileCopyMoveService;
            _stateManager = stateManager;
        }

        public async Task<SyncJobReport> RunSyncAsync(SyncJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            // Initialize job state
            var jobState = new SyncJobState();
            _stateManager.JobStates[job] = jobState;

            var touchedObjects = new List<WObject>();
            var ignoredItems = 0;

            // Step 1: Scan directory and find all matching files
            var scannedObjects = await _scannerService.ScanAsync(job);
            jobState.TotalItemToProcess = scannedObjects.Count;

            foreach (var wObject in scannedObjects)
            {
                try
                {
                    // Check if the file in source is newer than the file in destination (if it exists)
                    var destinationFilePath = Path.Combine(job.DstDirectory, wObject.Name);

                    // Only process files that are newer or do not exist at the destination
                    if (wObject is WFile file)
                    {
                        var destinationFileExists = File.Exists(destinationFilePath);
                        var shouldCopy = !destinationFileExists ||
                                         File.GetLastWriteTimeUtc(file.FullPath) > File.GetLastWriteTimeUtc(destinationFilePath);

                        if (shouldCopy)
                        {
                            await _fileCopyMoveService.CopyFileAsync(file.FullPath, destinationFilePath);
                            touchedObjects.Add(wObject);
                            jobState.ItemsProcessed++;
                        }
                        else
                        {
                            ignoredItems++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jobState._loglines.Add($"Failed to copy/move file '{wObject.Name}': {ex.Message}");
                }
            }

            // Step 2: Generate a report
            var report = new SyncJobReport
            {
                SyncJob = job,
                TouchedObjects = touchedObjects,
                DateTime = DateTime.UtcNow,
                IgnoredItems = ignoredItems
            };

            jobState._loglines.Add($"Sync job '{job.Name}' completed at {report.DateTime}. Processed {jobState.ItemsProcessed}/{jobState.TotalItemToProcess} items.");

            return report;
        }

        public async Task RunSyncAsync(List<SyncJob> jobs)
        {
            foreach (var job in jobs)
            {
                await RunSyncAsync(job);
            }
        }
    }
}
