using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Content;

namespace WSyncPro.Core.Services
{
    public interface ISyncService
    {
        public Task LoadJoblistFromFile(string joblistFilePath);
        public Task SaveJoblistToFile(string joblistFilePath);
        public Task<List<SyncJob>> GetAllJobs();
        public Task<SyncJob> GetJobById(int id);
        public Task AddJob(SyncJob job);
        public Task AddJob(List<SyncJob> jobs);
        public Task<(Guid jobId, int filesProcessed, string message)> RunAllEnabledJobs(); // runs syncjobs and returns ( jobid, numberOfFilesTouched, report )

    }
}
