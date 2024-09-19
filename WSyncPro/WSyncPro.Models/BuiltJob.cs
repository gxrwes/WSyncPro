using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models
{
    public class BuiltJob : Job
    {
        public List<string> FileList { get; set; } = new List<string>();
        public float TotalFileSizeInGB { get; set; } // the accumulated filesize of all files in filelist
    }
}
