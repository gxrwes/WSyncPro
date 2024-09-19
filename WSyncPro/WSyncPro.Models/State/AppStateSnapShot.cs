using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.State
{
    public class AppStateSnapShot
    {
        public Guid Guid { get; set; }
        public DateTime TimeStamp { get; set; } // created on init

        public Dictionary<Job, List<JobProgress>> JobStateList { get; set; }

    }
}
