using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Files
{
    public class WFile : WBaseItem
    {
        public string FileExtension { get; set; }
        public int FileSize { get; set; }
        public List<string> PreviousPathsList { get; set; } = new List<string>();
    }
}
