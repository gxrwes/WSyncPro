using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Reporting
{
    public class JobSummary
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public int TotalFiles { get; set; }
        public int FilesProcessed { get; set; }
        public int FilesFailed { get; set; }
        public TimeSpan JobTime { get; set; }
    }
}
