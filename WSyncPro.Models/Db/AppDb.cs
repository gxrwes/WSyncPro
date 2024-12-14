using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Models.Db
{
    public class AppDb
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Change SyncJobs to a property
        public List<SyncJob> SyncJobs { get; set; } = new List<SyncJob>();

        // Change dbName to a property (optional if you want it serialized)
        public string DbName { get; set; } = "default";
        public HashSet<string> GeneratedGuids { get; set; } = new HashSet<string>();
    }
}

