
namespace WSyncPro.Models.Jobs
{
    public class JobExecution
    {
        public Guid Id { get; set; }
        public string JobId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public List<JobExecution> Executions { get; set; } = new List<JobExecution>();
        public JobStatus Status { get; set; }
    }

}
