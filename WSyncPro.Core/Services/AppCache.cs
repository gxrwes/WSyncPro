using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Db;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public class AppCache : IAppCache
    {
        private readonly IAppLocalDb _localDb;
        private readonly ILogger<AppCache> _logger;
        private readonly AppDb _cache;

        public AppCache(IAppLocalDb localDb, ILogger<AppCache> logger)
        {
            _localDb = localDb;
            _logger = logger;
            _cache = new AppDb();
        }

        public async Task<bool> AddSyncJob(SyncJob job)
        {
            if (_cache.SyncJobs.Exists(j => j.Id == job.Id))
            {
                _logger.LogWarning("Sync job with ID {JobId} already exists", job.Id);
                return false;
            }

            _cache.SyncJobs.Add(job);
            _logger.LogInformation("Sync job with ID {JobId} added", job.Id);
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

        public async Task<bool> SyncWithDb()
        {
            try
            {
                bool success = await _localDb.LoadDb();
                if (!success)
                {
                    _logger.LogWarning("Failed to load database for syncing");
                    return false;
                }

                var appDb = await _localDb.GetAppDb();
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
    }
}
