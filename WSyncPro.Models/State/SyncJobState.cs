using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.State
{
    public class SyncJobState
    {
        public int TotalItemToProcess;
        public float ItemsProcessed;
        public List<string> _loglines = new List<string>();
    }
}
