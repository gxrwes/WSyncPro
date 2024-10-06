using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Data
{
    public class WFileMetadata
    {
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>(); // addinional metadata
    }
}
