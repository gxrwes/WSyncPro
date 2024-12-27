using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Settings;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Test.Mock
{
    public class MockAppCache : IAppCache
    {
        private readonly List<SyncJob> _syncJobs = new();
        private readonly List<CopyJob> _copyJobs = new();
        private readonly List<FileHistorySnapShot> _snapshots = new();
        private readonly List<JobExecution> _executions = new();
        private readonly List<WDirectory> _directories = new();

        public Task<bool> AddSyncJob(SyncJob job)
        {
            _syncJobs.Add(job);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveSyncJob(string jobId)
        {
            _syncJobs.RemoveAll(job => job.Id.ToString() == jobId);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveSyncJob(SyncJob job)
        {
            _syncJobs.Remove(job);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateSyncJob(SyncJob job)
        {
            RemoveSyncJob(job.Id.ToString());
            _syncJobs.Add(job);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateSyncJob(string jobId)
        {
            var job = _syncJobs.FirstOrDefault(j => j.Id.ToString() == jobId);
            if (job != null) return UpdateSyncJob(job);
            return Task.FromResult(false);
        }

        public Task<List<SyncJob>> GetAllSyncJobs() => Task.FromResult(_syncJobs);

        public Task<SyncJob> GetSyncJob(string jobId) => Task.FromResult(_syncJobs.FirstOrDefault(job => job.Id.ToString() == jobId));

        public Task<bool> CacheUpdated() => Task.FromResult(true);

        public Task<bool> SyncWithDb() => Task.FromResult(true);

        public Task AddCopyJob(CopyJob job)
        {
            _copyJobs.Add(job);
            return Task.CompletedTask;
        }

        public Task AddFileHistorySnapshot(FileHistorySnapShot snapshot)
        {
            _snapshots.Add(snapshot);
            return Task.CompletedTask;
        }

        public Task AddJobExecution(JobExecution execution)
        {
            _executions.Add(execution);
            return Task.CompletedTask;
        }

        public Task AddDirectory(WDirectory directory)
        {
            _directories.Add(directory);
            return Task.CompletedTask;
        }

        public Task<AppSettingsModel> GetAppSettings() => Task.FromResult(new AppSettingsModel());

        public Task<bool> SetAppSettings(AppSettingsModel newSettings)
        {
            return Task.FromResult(true);
        }

        public Task<List<WDirectory>> GetDirectories() => Task.FromResult(_directories);

        public Task<List<CopyJob>> GetCopyJobs() => Task.FromResult(_copyJobs);

        public Task<List<FileHistorySnapShot>> GetFileHistorySnapShots() => Task.FromResult(_snapshots);

        public Task<List<JobExecution>> GetJobExecutions() => Task.FromResult(_executions);

        public Guid GetUUID()
        {
            throw new NotImplementedException();
        }

        List<WDirectory> IAppCache.GetDirectories()
        {
            throw new NotImplementedException();
        }

        List<CopyJob> IAppCache.GetCopyJobs()
        {
            throw new NotImplementedException();
        }

        List<FileHistorySnapShot> IAppCache.GetFileHistorySnapShots()
        {
            throw new NotImplementedException();
        }

        List<JobExecution> IAppCache.GetJobExecutions()
        {
            throw new NotImplementedException();
        }
    }
}
