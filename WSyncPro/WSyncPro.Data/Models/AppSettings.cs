using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Data.Models
{
    public class AppSettings
    {
        public int AppLaunchCounter = 0; // counts how many times the app has been launched
        public string StateFilePath { get; set; } = "AppState.json";
        public string LogFilePath { get; set; } = "WSyncPro_Log.txt";

        public bool SetupEnabled = true;

        public string TrashDirectory = "";

        public string HandBrakeCliPath = "";

        public string DefaultArchiveDirectory = "";
    }
}
