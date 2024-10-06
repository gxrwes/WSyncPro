using System;
using System.Collections.Generic;
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

        // Add the missing property
        public bool Selected { get; set; } // Indicates whether the job is selected in the UI
    }
}
