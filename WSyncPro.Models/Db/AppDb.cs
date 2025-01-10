using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Import;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Settings;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Models.Db
{
    public class AppDb
    {
        public DateTime LastUpdate { get; set; } = DateTime.MinValue;
        public Guid Id { get; set; } = Guid.NewGuid();

        // Change SyncJobs to a property
        public List<SyncJob> SyncJobs { get; set; } = new List<SyncJob>();
        public List<CopyJob> CopyJobs { get; set; } = new List<CopyJob>();// Holds all copy jobs that are created
        public List<FileHistorySnapShot> fileHistorySnapShots { get; set; } = new List<FileHistorySnapShot>(); // holds all versioning snapshots that are creaed
        public List<JobExecution> JobExecutions { get; set; } = new List<JobExecution>();// holds all jobexecutions
        public List<WDirectory> AllDirectories { get; set; } = new List<WDirectory>();// chaes all WDirectory items ( and their child content ) for quid access when we have already scanned a directory in the syncservice

        // Change dbName to a property (optional if you want it serialized)
        public string DbName { get; set; } = "default";
        public HashSet<string> GeneratedGuids { get; set; } = new HashSet<string>();
        public AppSettingsModel Appsettings { get; set; } = new AppSettingsModel();

    }
}

