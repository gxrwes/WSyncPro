using System;
using System.Collections.Generic;
using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Content
{
    public class SyncJob
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; } = Status.Undefined;
        public string SrcDirectory { get; set; }
        public string DstDirectory { get; set; }
        public List<string> FilterInclude { get; set; } = new List<string>();
        public List<string> FilterExclude { get; set; } = new List<string>();

        public bool includeDirectories { get; set; } = true;
        public bool Enabled { get; set; } = false;// Indicates whether the job is selected in the UI
    }
}
