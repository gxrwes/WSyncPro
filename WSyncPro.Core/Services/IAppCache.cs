using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Jobs;

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

        public Task<bool> SyncWithDb();
    }
}
