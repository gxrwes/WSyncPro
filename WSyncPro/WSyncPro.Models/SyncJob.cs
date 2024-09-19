using System.Collections.Generic;

namespace WSyncPro.Models
{
    /// <summary>
    /// Represents a synchronization job with additional properties.
    /// Inherits from the Job class.
    /// </summary>
    public class SyncJob : Job
    {
        public string SrcDirectory { get; set; }
        public string DstDirectory { get; set; }
        public int FilesToSync { get; set; }
        public int TotalFilesSynced { get; set; }
        public int FailedFiles { get; set; }
        public List<string> InclWilcardString { get; set; } = new List<string>();
        public List<string> ExclWildcardString { get; set; } = new List<string>();
    }
}
