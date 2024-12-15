using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public interface ISyncService
    {
        public Task<List<CopyJob>> CreateCpJobsForSyncJobs(List<SyncJob> jobs);
        public Task<List<CopyJob>> CreateCpJobsForSyncJob(SyncJob jobs);
        public Task<List<bool>> VerifyCopyJobsExecuted(List<CopyJob> jobs);
        public Task<WDirectory> ScanDirectoryAsync(string path);
    }
}
