// AppStateSnapShot.cs
using System;
using System.Collections.Generic;

namespace WSyncPro.Models.State
{
    /// <summary>
    /// Represents a snapshot of the application's state at a specific point in time.
    /// </summary>
    public class AppStateSnapShot
    {
        /// <summary>
        /// Gets or sets the unique identifier of the snapshot.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the snapshot was created.
        /// </summary>
        public DateTime TimeStamp { get; set; } // created on init

        /// <summary>
        /// Gets or sets the list of Jobs.
        /// </summary>
        public List<Job> Jobs { get; set; } = new List<Job>();

        /// <summary>
        /// Gets or sets the mapping of Job IDs to their respective progress histories.
        /// </summary>
        public Dictionary<string, List<JobProgress>> JobStateList { get; set; } = new Dictionary<string, List<JobProgress>>();
    }
}
