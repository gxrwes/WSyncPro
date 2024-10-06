using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.User;

namespace WSyncPro.Models.Data
{
    public class WObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string FullPath { get; set; }

        public WFileMetadata WFileMetadata { get; set; } = new WFileMetadata();
        public Client OriginatingClient { get; set; } = new Client();
    }
}
