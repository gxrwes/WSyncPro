using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.User;

namespace WSyncPro.Util.System
{
    public static class GetClientInfo
    {
        public static Client GetClientSystemInformation()
        {
            return new Client
            {
                Id = Guid.NewGuid(),
                Name = Environment.MachineName,
                MachineId = GetMachineId(),
                Ip = GetLocalIPAddress(),
                OperatingSystem = GetOperatingSystemInfo()
            };
        }

        public static string GetMachineId()
        {
            // Placeholder for machine ID retrieval logic. Can be implemented using WMI or other unique machine identifiers.
            return Guid.NewGuid().ToString(); // Replace this with actual machine-specific ID.
        }

        public static string GetLocalIPAddress()
        {
            string localIP = "Not Available";
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipProperties = ni.GetIPProperties();
                        foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                localIP = ip.Address.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Handle any exceptions related to network access.
            }

            return localIP;
        }

        public static OperatingSystem GetOperatingSystemInfo()
        {
            return Environment.OSVersion;
        }
    }
}
