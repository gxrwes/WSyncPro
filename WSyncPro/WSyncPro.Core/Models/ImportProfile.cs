using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Models
{
    public class ImportProfile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultImportPath { get; set; }
        public string[] TargetedFileTypes { get; set; }
        public string[] FilterStrings { get; set; }
        public string[] AntiFilterStrings { get; set; }

        public string GroupingPattern { get; set; }
    }
}
