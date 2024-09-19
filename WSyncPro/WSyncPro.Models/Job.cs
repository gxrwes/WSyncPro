using System;

namespace WSyncPro.Models
{
    /// <summary>
    /// Represents a generic job with basic properties.
    /// </summary>
    public class Job
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastRun { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
    }
}
