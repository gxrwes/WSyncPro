using System;

namespace WSyncPro.Core.Models
{
    public enum JobType
    {
        Sync,
        Archive,
        Clean,
        ReRender
    }

    public class Job
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; } // Not required for ReRender
        public string[] TargetedFileTypes { get; set; }
        public string[] FilterStrings { get; set; }
        public string[] AntiFilterStrings { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsScheduled { get; set; }
        public Schedule Schedule { get; set; }
        public JobType JobType { get; set; }

        // Archive and ReRender specific properties
        public ArchiveOptions ArchiveOptions { get; set; }
        public ReRenderOptions ReRenderOptions { get; set; }
    }

    public class ArchiveOptions
    {
        public string GroupingPattern { get; set; }
    }

    public class ReRenderOptions
    {
        public string Preset { get; set; }
        public string AdvancedOptions { get; set; }
    }

    public class Schedule
    {
        public DateTime NextRunTime { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
