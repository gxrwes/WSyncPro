using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Jobs
{
    public enum JobStatus
    {
        Unknown,
        New,
        Idle,
        Running,
        Finishing,
        Successful,
        Failed,
        Crashed,
        Canceled

    }
}
