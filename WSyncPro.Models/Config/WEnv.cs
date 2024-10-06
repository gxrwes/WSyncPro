using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Config
{
    public class WEnv
    {
        public Dictionary<string, string> EnvVars { get; set; } = new Dictionary<string, string>();
    }
}
