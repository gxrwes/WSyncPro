using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Data;

namespace WSyncPro.Models.Content
{
    public class SyncJobReport
    {
        public SyncJob SyncJob { get; set; }
        public List<WObject> TouchedObjects { get; set; }
        public DateTime DateTime { get; set; }
        public int IgnoredItems { get; set; }
    }
}
