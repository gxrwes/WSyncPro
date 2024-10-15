using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Data
{
    public class WDirectory : WObject
    {
        public string DirectoryAbsolutePath; // the absolute path of the filesystem
        public string DirectoryName;
        public float DirectorySize;
        public FileSizeMultiplyer FilesizeMultiplyer;
    }
}
