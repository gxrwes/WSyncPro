using System.Collections.Generic;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Managers
{
    public interface IStateManager
    {
        // Exposes the JobStates dictionary
        Dictionary<SyncJob, SyncJobState> JobStates { get; set; }

        public Task TryLoadJobsFromFile();
        public Task TrySaveJobsToFile();

        public Task<Dictionary<SyncJob, SyncJobState>> GetAllJobStates();
        public Task<List<SyncJob>> GetAllJobs(); // returns all jobs without state

        public Task UpdateJob(SyncJob job); // Updates a Job in the JobStates dicionary
        public Task AddJob(SyncJob job); // Adds a Job
        public Task RemoveJob(SyncJob job);

    }
}
