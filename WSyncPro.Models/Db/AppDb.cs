using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Models.Db
{
    public class AppDb
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Change SyncJobs to a property
        public List<SyncJob> SyncJobs { get; set; } = new List<SyncJob>();
        public List<CopyJob> CopyJobs { get; set; } // Holds all copy jobs that are created
        public List<FileHistorySnapShot> fileHistorySnapShots { get; set; } // holds all versioning snapshots that are creaed
        public List<JobExecution> JobExecutions { get; set; } // holds all jobexecutions
        public List<WDirectory> AllDirectories { get; set; } // chaes all WDirectory items ( and their child content ) for quid access when we have already scanned a directory in the syncservice

        // Change dbName to a property (optional if you want it serialized)
        public string DbName { get; set; } = "default";
        public HashSet<string> GeneratedGuids { get; set; } = new HashSet<string>();
    }
}

