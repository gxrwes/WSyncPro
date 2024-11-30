using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Files
{
    public class WDirectory : WBaseItem
    {
        public List<WBaseItem> Items { get; set; }
    }
}
