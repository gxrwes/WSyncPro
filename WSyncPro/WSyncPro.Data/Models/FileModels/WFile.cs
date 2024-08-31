using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Data.Models.FileModels
{
    public class WFile
    {
        public string Name { get; set; } // with extension
        public string Path { get; set; }
        public int id { get; set; }
        public FileType FileType { get; set; }

        public float FileSizeInMB { get; set; }
    }
}
