using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Data
{
    public class WFile : WObject
    {
        public string Filetype;
        public float Filesize;
        public FileSizeMultiplyer FilesizeMultiplyer;
    }
}
