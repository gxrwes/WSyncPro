using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Reporting
{
    public class SyncSummary
    {
        public int TotalJobs { get; set; }
        public int TotalJobsCompleted { get; set; }
        public int TotalFilesProcessed { get; set; }
        public int TotalFilesFailed { get; set; }
        public TimeSpan TotalTime { get; set; }
        public List<JobSummary> JobSummaries { get; set; } = new List<JobSummary>();
    }
}
