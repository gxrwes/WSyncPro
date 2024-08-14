using System;
using System.Collections.Generic;
using System.IO;

namespace WSyncPro.Core.Services
{
    public class ExternalDeviceDetectorService
    {
        public List<UsbDeviceInfo> GetExternalUsbDevices()
        {
            var usbDevices = new List<UsbDeviceInfo>();

            // Get all the drives on the system
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                // Check if the drive is a removable USB drive and is ready
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    var usbDeviceInfo = new UsbDeviceInfo(
                        drive.Name,
                        drive.VolumeLabel,
                        drive.TotalSize,
                        drive.AvailableFreeSpace,
                        drive.DriveFormat
                    );

                    usbDevices.Add(usbDeviceInfo);
                }
            }

            return usbDevices;
        }
    }
}
