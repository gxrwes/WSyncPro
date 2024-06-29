using System;
using System.Collections.Generic;

namespace WSyncProApp.Models
{
    using System;
    using System.Collections.Generic;

    namespace WSyncProApp.Models
    {
        public class Job
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public int Priority { get; set; }
            public string Description { get; set; }
            public List<string> TargetedFiletypes { get; set; }
            public string Filter { get; set; }
            public DateTime LastRunDate { get; set; }
            public List<string> History { get; set; }
            public string SourceDirectory { get; set; }
            public string TargetDirectory { get; set; }
            public string Mode { get; set; } // Archive, Sync, Publish
            public bool IsActive { get; set; } // New property
        }
    }

}
