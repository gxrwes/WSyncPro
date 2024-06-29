using System;

namespace WSyncProApp.Models
{
    public class AppSettings
    {
        public bool DevMode { get; set; }
        public string TrashDirectory { get; set; }
        public string RunHistorySaveLocation { get; set; }
        public string SaveRunsFor { get; set; }
    }
}
