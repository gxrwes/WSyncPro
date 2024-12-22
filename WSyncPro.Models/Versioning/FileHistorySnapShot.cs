using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Versioning
{
    // (before, after)
    public class FileHistorySnapShot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public (DateTime, DateTime) TimeStamp { get; set; }
        public (double, double) Filesize { get; set; }
        public (string, string) Filename { get; set; }
        public (string, string) FilePath { get; set; }
        public (string, string) LastEdited { get; set; }
        public (string, string) TriggerJobId { get; set; }
    }
}
