using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Config;
using WSyncPro.Models.User;

namespace WSyncPro.Models.State
{
    public class AppState
    {
        public string Version { get; set; }
        public Client CurrentUser { get; set; } = new Client();

        public WEnv Env { get; set; } = new WEnv();

    }
}
