using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models
{
    public class SettingsModel
    {
        public string DefaultImportPath { get; set; } = string.Empty;
        public string TrashDirecotry { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string About { get; set; }

    }
}
