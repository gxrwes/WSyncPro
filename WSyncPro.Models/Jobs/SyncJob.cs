using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Filter;

namespace WSyncPro.Models.Jobs
{
    public class SyncJob : Job
    {
        public string SrcDirectory { get; set; }
        public string DstDirectory { get; set; }
        public bool KeepDirectories { get; set; }
        public FilterParams FilterParams { get; set; }
        public int RunCount { get; set; } = 0;
        public JobStatus Status { get; set; } = JobStatus.Unknown;
    }
}
