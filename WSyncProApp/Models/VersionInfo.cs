using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncProApp.Models
{
    public class ReleaseNote
    {
        public string Version { get; set; }
        public string Notes { get; set; }
    }

    public class VersionInfo
    {
        public string CurrentVersion { get; set; }
        public string LastReleaseNotes { get; set; }
        public List<ReleaseNote> ReleaseNotesHistory { get; set; }
    }
}
