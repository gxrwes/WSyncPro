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
        public List<SyncJob> SyncJobs = new List<SyncJob>();
        private string dbName = "default";

    }
}
