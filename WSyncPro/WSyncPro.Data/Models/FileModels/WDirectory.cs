using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Data.Models.FileModels
{
    public class WDirectory : WFile
    {
        public WDirectory()
        {
            FileType = FileType.Directory;
            Files = new List<WFile>(); // Ensure this list is always initialized
        }

        public List<WFile> Files { get; set; }
    }
}
