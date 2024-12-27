using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace WSyncPro.Util.System
{
    public static class ExternalDriveDetector
    {
        /// <summary>
        /// Detects all external drives connected to the system and returns a list of ExternalDriveModel.
        /// </summary>
        /// <returns>List of ExternalDriveModel representing the connected external drives.</returns>
        public static List<ExternalDriveModel> GetExternalDrives()
        {
            var drives = new List<ExternalDriveModel>();

            try
            {
                // Query WMI for drive information
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE MediaType = 'Removable Media'"))
                {
                    foreach (var drive in searcher.Get())
                    {
                        var deviceId = drive["DeviceID"]?.ToString();
                        var partitions = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition");

                        foreach (var partition in partitions.Get())
                        {
                            var logicalDisks = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass=Win32_LogicalDiskToPartition");

                            foreach (var logicalDisk in logicalDisks.Get())
                            {
                                var devicePath = logicalDisk["DeviceID"]?.ToString();

                                var driveInfo = new DriveInfo(devicePath);
                                drives.Add(new ExternalDriveModel
                                {
                                    Enabled = driveInfo.IsReady,
                                    DeviceName = drive["Model"]?.ToString(),
                                    DeviceSize = FormatSize((ulong)(drive["Size"] ?? 0)),
                                    DeviceType = "Removable",
                                    DeviceVersion = drive["FirmwareRevision"]?.ToString(),
                                    DevicePath = driveInfo.RootDirectory.FullName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error detecting external drives: {ex.Message}");
            }

            return drives;
        }

        /// <summary>
        /// Formats the size in bytes into a human-readable string (e.g., KB, MB, GB).
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes.</param>
        /// <returns>Formatted size string.</returns>
        private static string FormatSize(ulong sizeInBytes)
        {
            const double KB = 1024;
            const double MB = KB * 1024;
            const double GB = MB * 1024;

            if (sizeInBytes >= GB)
                return $"{sizeInBytes / GB:F2} GB";
            if (sizeInBytes >= MB)
                return $"{sizeInBytes / MB:F2} MB";
            if (sizeInBytes >= KB)
                return $"{sizeInBytes / KB:F2} KB";

            return $"{sizeInBytes} Bytes";
        }
    }

    public class ExternalDriveModel
    {
        public bool Enabled { get; set; }
        public string DeviceName { get; set; }
        public string DeviceSize { get; set; }
        public string DeviceType { get; set; }
        public string DeviceVersion { get; set; }
        public string DevicePath { get; set; }
    }
}
