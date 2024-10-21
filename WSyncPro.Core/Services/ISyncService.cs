using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Reporting;

namespace WSyncPro.Core.Services
{
    // Progress event arguments class moved outside of SyncService for accessibility
    public class ProgressChangedEventArgs : EventArgs
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public int TotalJobs { get; set; }
        public int CurrentJobIndex { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public string Message { get; set; }
    }

    public interface ISyncService
    {
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        Task LoadJoblistFromFile(string joblistFilePath);
        Task SaveJoblistToFile(string joblistFilePath);
        Task<List<SyncJob>> GetAllJobs();
        Task<SyncJob> GetJobById(Guid id);
        Task AddJob(SyncJob job);
        Task AddJob(List<SyncJob> jobs);
        Task<SyncSummary> RunAllEnabledJobs();
    }
}
