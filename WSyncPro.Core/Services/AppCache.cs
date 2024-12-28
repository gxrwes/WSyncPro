using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Db;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Settings;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    public class AppCache : IAppCache
    {
        private readonly IAppLocalDb _localDb;
        private readonly ILogger<AppCache> _logger;
        private readonly AppDb _cache = new AppDb();
        private bool _update = false;

        public AppCache(IAppLocalDb localDb, ILogger<AppCache> logger)
        {
            _localDb = localDb;
            _logger = logger;
            var loadedFiles = _localDb.GetAppDb();
            if (loadedFiles != null) _cache = loadedFiles;
            else
                _cache = new AppDb();
        }

        public async Task<bool> AddSyncJob(SyncJob job)
        {
            // Generate UUID
            job.Id = GetUUID();

            if (_cache.SyncJobs.Exists(j => j.Id == job.Id))
            {
                _logger.LogWarning("Sync job with ID {JobId} already exists", job.Id);
                return false;
            }

            _cache.SyncJobs.Add(job);
            _logger.LogInformation("Sync job with ID {JobId} added", job.Id);
            _update = true;
            return await _localDb.UpdateDb(_cache);
        }

        public async Task<bool> RemoveSyncJob(string jobId)
        {
            var job = _cache.SyncJobs.FirstOrDefault(j => j.Id.ToString() == jobId);
            if (job == null)
            {
                _logger.LogWarning("Sync job with ID {JobId} not found", jobId);
                return false;
            }

            _cache.SyncJobs.Remove(job);
            _logger.LogInformation("Sync job with ID {JobId} removed", jobId);
            _update = true;
            return await _localDb.UpdateDb(_cache);
        }

        public Task<bool> RemoveSyncJob(SyncJob job)
        {
            return RemoveSyncJob(job.Id.ToString());
        }

        public async Task<bool> UpdateSyncJob(SyncJob job)
        {
            var existingJob = _cache.SyncJobs.FirstOrDefault(j => j.Id.ToString() == job.Id.ToString());
            if (existingJob == null)
            {
                _logger.LogWarning("Sync job with ID {JobId} not found for update", job.Id.ToString());
                return false;
            }

            _cache.SyncJobs.Remove(existingJob);
            _cache.SyncJobs.Add(job);
            _logger.LogInformation("Sync job with ID {JobId} updated", job.Id);
            return await _localDb.UpdateDb(_cache);
        }

        public async Task<bool> UpdateSyncJob(string jobId)
        {
            var job = _cache.SyncJobs.FirstOrDefault(j => j.Id.ToString() == jobId);
            if (job == null)
            {
                _logger.LogWarning("Sync job with ID {JobId} not found for update", jobId);
                return false;
            }

            return await UpdateSyncJob(job);
        }

        public async Task<bool> SyncWithDb() // currently we reload and check if that is succesful to be able to sync
        {
            try
            {
                bool success = await _localDb.LoadDb();
                if (!success)
                {
                    _logger.LogWarning("Failed to load database for syncing");
                    return false;
                }

                var appDb = await _localDb.GetAppDbAsync();
                _cache.SyncJobs.Clear();
                _cache.SyncJobs.AddRange(appDb.SyncJobs);
                _logger.LogInformation("Cache synced with database successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing cache with database");
                return false;
            }
        }

        public async Task<List<SyncJob>> GetAllSyncJobs()
        {
            try
            {
                _update = false;
                return _cache.SyncJobs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Returning SyncJobs From Cache");
                return new List<SyncJob>();
            }
        }

        public async Task<SyncJob> GetSyncJob(string jobId)
        {
            try
            {
                SyncJob sJ = _cache.SyncJobs.FirstOrDefault(_ => _.Id.ToString() == jobId);
                if (sJ != null) return sJ;

                throw new Exception("no SyncJob Found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing cache with database");
                return new SyncJob();
            }
        }

        public async Task<bool> CacheUpdated()
        {
            return _update;
        }

        public Guid GetUUID()
        {
            try
            {
                // Get the UUID as a string from the database
                string uuidString = _localDb.GetUUID();

                // Convert the string to a Guid and return it
                return Guid.Parse(uuidString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting UUID string to Guid");
                throw;
            }
        }
        public async Task AddCopyJob(CopyJob job)
        {
            try
            {
                if (job == null)
                    throw new ArgumentNullException(nameof(job));
                if (_cache.CopyJobs == null) _cache.CopyJobs = new List<CopyJob>();
                if (!_cache.CopyJobs.Any(j => j.Guid == job.Guid))
                {
                    _cache.CopyJobs.Add(job);
                    _update = true;
                    _logger.LogInformation("Copy job with ID {JobId} added", job.Guid);
                    await _localDb.UpdateDb(_cache);
                }
                else
                {
                    _logger.LogWarning("Copy job with ID {JobId} already exists", job.Guid);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding copy job to cache");
                throw ex;
            }
        }

        public async Task AddFileHistorySnapshot(FileHistorySnapShot snapshot)
        {
            try
            {
                if (snapshot == null)
                    throw new ArgumentNullException(nameof(snapshot));
                if (_cache.fileHistorySnapShots == null) _cache.fileHistorySnapShots = new List<FileHistorySnapShot>();
                _cache.fileHistorySnapShots.Add(snapshot);
                _update = true;
                _logger.LogInformation("FileHistorySnapShot with ID {SnapShotId} added", snapshot.Id);
                await _localDb.UpdateDb(_cache);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding file history snapshot to cache");
                throw;
            }
        }

        public async Task AddJobExecution(JobExecution execution)
        {
            try
            {
                if (execution == null)
                    throw new ArgumentNullException(nameof(execution));
                if(_cache.JobExecutions == null) _cache.JobExecutions = new List<JobExecution>();
                _cache.JobExecutions.Add(execution);
                _update = true;
                _logger.LogInformation("JobExecution with ID {ExecutionId} added", execution.Id);
                await _localDb.UpdateDb(_cache);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding job execution to cache");
                throw;
            }
        }

        public async Task AddDirectory(WDirectory directory)
        {
            try
            {
                if (directory == null)
                    throw new ArgumentNullException(nameof(directory));

                if (_cache.AllDirectories == null) _cache.AllDirectories = new List<WDirectory>();

                if (!_cache.AllDirectories.Any(d => d.Id == directory.Id))
                {
                    _cache.AllDirectories.Add(directory);
                    _update = true;
                    _logger.LogInformation("Directory with ID {DirectoryId} added", directory.Id);
                    await _localDb.UpdateDb(_cache);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding directory to cache");
                throw;
            }
        }

        // Additional helper methods to retrieve specific lists
        public List<WDirectory> GetDirectories() => _cache.AllDirectories;

        public List<CopyJob> GetCopyJobs() => _cache.CopyJobs;

        public List<FileHistorySnapShot> GetFileHistorySnapShots()
        {
            if(_cache.fileHistorySnapShots == null) _cache.fileHistorySnapShots = new List<FileHistorySnapShot>();
            return  _cache.fileHistorySnapShots;
        } 

        public List<JobExecution> GetJobExecutions() => _cache.JobExecutions;

        public async Task<AppSettingsModel> GetAppSettings()
        {
            if(_cache.Appsettings == null) _cache.Appsettings = new AppSettingsModel();
            return _cache.Appsettings;
        }

        public async Task<bool> SetAppSettings(AppSettingsModel newSettings)
        {
            try
            {
                if(newSettings == null) throw new ArgumentNullException(nameof(newSettings));
                if(newSettings.AppSettingsBackupPath != null && newSettings.AppSettingsBackupPath.Any())
                {
                    _cache.Appsettings.AppSettingsBackupPath = newSettings.AppSettingsBackupPath;
                    _logger.LogInformation("Set New Backup Path");
                }
                if(newSettings.ImportDefault != null)
                {
                    _cache.Appsettings.ImportDefault = newSettings.ImportDefault;
                    _logger.LogInformation("Set New Import Defaults");

                }
                await _localDb.UpdateDb(_cache);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong updating settings in AppCache with :" + ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateCopyJob(CopyJob job)
        {
            try
            {
                // Find the job in the cache by Guid
                var existingJob = _cache.CopyJobs.FirstOrDefault(j => j.Guid == job.Guid);

                if (existingJob == null)
                {
                    // Job not found
                    return false;
                }

                // Update the existing job's properties
                existingJob.SrcFilePathAbsolute = job.SrcFilePathAbsolute;
                existingJob.DstFilePathAbsolute = job.DstFilePathAbsolute;
                existingJob.Timestamp = job.Timestamp;
                existingJob.Overwrite = job.Overwrite;
                existingJob.Successful = job.Successful;
                existingJob.Status = job.Status;
                await _localDb.UpdateDb(_cache);

                return (true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Updating CopyJob went wrong with: " + ex.Message);
                // Log the exception if logging is available
                Console.WriteLine($"Error updating CopyJob: {ex.Message}");
                return false;
            }
        }

    }
}
