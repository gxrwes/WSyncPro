using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Settings;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    // Holds AppState in memory and interacts with IAppLocalDb to sync db state -> this service needs to be registered as singelton
    public interface IAppCache
    {
        public Task<bool> AddSyncJob(SyncJob job);
        public Task<bool> RemoveSyncJob(string jobId);
        public Task<bool> RemoveSyncJob(SyncJob job);
        public Task<bool> UpdateSyncJob(SyncJob job);
        public Task<bool> UpdateSyncJob(string jobId);
        public Task<List<SyncJob>> GetAllSyncJobs();
        public Task<SyncJob> GetSyncJob(string jobId);
        public Task<bool> CacheUpdated();

        public Task<bool> SyncWithDb();
        // To get a unique id, usually done by db
        public Guid GetUUID();
        public Task AddCopyJob(CopyJob job);

        public Task AddFileHistorySnapshot(FileHistorySnapShot snapshot);
        public Task AddJobExecution(JobExecution execution);
        public Task AddDirectory(WDirectory directory);

        // Additional helper methods to retrieve specific lists
        public List<WDirectory> GetDirectories();

        public List<CopyJob> GetCopyJobs();

        public List<FileHistorySnapShot> GetFileHistorySnapShots();

        public List<JobExecution> GetJobExecutions();
        public Task<AppSettingsModel> GetAppSettings();
        public Task<bool> SetAppSettings(AppSettingsModel newSettings);
        public Task<bool> UpdateCopyJob(CopyJob job);
    }
}
