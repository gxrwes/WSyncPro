using System;
using System.Collections.Generic;

namespace WSyncPro.Core.Models
{
    public class SyncRun
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long DataTransferred { get; set; }
        public long DestinationSizeBeforeRun { get; set; }
        public long DestinationSizeAfterRun { get; set; }
        public int FilesTransferred { get; set; }
        public int DirectoriesCreated { get; set; }
        public Job RelatedJob { get; set; }
        public List<SyncFileMetadata> Files { get; set; } = new List<SyncFileMetadata>();
        public string Log { get; set; } // Log details of the run
    }

    public class SyncFileMetadata
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
    }
}
