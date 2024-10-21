using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Reporting;

namespace WSyncPro.Models.Content
{
    public class ImportReport
    {
        public int TotalFilesImportet { get; set; }
        public int TotalFilesFailed { get; set; }
        public TimeSpan TotalTime { get; set; }
        public List<JobSummary> JobSummaries { get; set; } = new List<JobSummary>();

    }
}
