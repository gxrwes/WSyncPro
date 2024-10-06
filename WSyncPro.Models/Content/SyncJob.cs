using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Content
{
    public class SyncJob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string SrcDirectory { get; set; }
        public string DstDirectory { get; set; }
        public List<string> FilterInclude { get; set; }
        public List<string> FilterExclude { get; set; }
    }
}
