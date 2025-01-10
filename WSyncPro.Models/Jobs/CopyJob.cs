using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Jobs
{
    public class CopyJob
    {
        public Guid Guid { get; set; }
        public string SrcFilePathAbsolute { get; set; }
        public string DstFilePathAbsolute { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Overwrite { get; set; }
        public bool Successful { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Unknown;
    }

    public enum CopyJobOperation
    {
        Copy,
        Move,
        Delete
    }
}
