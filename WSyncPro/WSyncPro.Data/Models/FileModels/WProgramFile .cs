using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Data.Models.FileModels
{
    public class WProgramFile : WFile
    {
        public string Version { get; set; }
        public string Vendor { get; set; }
        public WProgramFile()
        {
            FileType = FileType.Program;
        }
    }
}
