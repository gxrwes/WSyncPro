using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Models
{
    public class AppState
    {
        public List<Job> Jobs { get; set; } //Cached Jobs
        public List<SyncRun> SyncRuns { get; set; } //Cached SyncRuns
        public List<WDirectory> WDirectories { get; set; } //Cached Directories

        public List<String> LogLines { get; set; }
    }
}
