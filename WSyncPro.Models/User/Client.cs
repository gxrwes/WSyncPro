using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.User
{
    public class Client
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; } = string.Empty;
        public string MachineId { get; set; }
        public string Ip { get; set; }
        public OperatingSystem OperatingSystem { get; set; }

    }
}
