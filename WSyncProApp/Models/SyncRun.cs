using System;
using System.Collections.Generic;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Models
{
    public class SyncRun
    {
        public List<Job> Jobs { get; set; }
        public Dictionary<Guid, List<string>> JobHistories { get; set; } // Placeholder for job history
        public DateTime TimestampStart { get; set; }
        public DateTime TimestampCompletion { get; set; }
        public List<string> RunMessages { get; set; }

        public SyncRun()
        {
            Jobs = new List<Job>();
            JobHistories = new Dictionary<Guid, List<string>>();
            RunMessages = new List<string>();
        }
    }
}
