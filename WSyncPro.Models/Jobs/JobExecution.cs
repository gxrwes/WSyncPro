
using WSyncPro.Models.Versioning;

namespace WSyncPro.Models.Jobs
{
    public class JobExecution
    {
        public Guid Id { get; set; }
        public string JobId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public List<FileHistorySnapShot> Executions { get; set; } = new List<FileHistorySnapShot>();
        public JobStatus Status { get; set; }

        public JobExecution()
        {
            Id = Guid.NewGuid();
            JobId = "NotSet";
            Status = JobStatus.Unknown;
        }
        public JobExecution(string JobId, JobStatus jobStatus, List<FileHistorySnapShot> snapshots)
        {
            Id = Guid.NewGuid();
            Status = jobStatus;
            Executions = snapshots;

        }
    }

}
