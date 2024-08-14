using System.IO;

namespace WSyncPro.Core.Services
{
    public class UsbDeviceInfo
    {
        public string Name { get; set; }        // Drive name (e.g., "E:\")
        public string VolumeLabel { get; set; } // The label of the USB drive
        public long TotalSize { get; set; }     // Total size of the drive in bytes
        public long FreeSpace { get; set; }     // Free space on the drive in bytes
        public string FileSystem { get; set; }  // File system type (e.g., NTFS, FAT32)

        public UsbDeviceInfo(string name, string volumeLabel, long totalSize, long freeSpace, string fileSystem)
        {
            Name = name;
            VolumeLabel = volumeLabel;
            TotalSize = totalSize;
            FreeSpace = freeSpace;
            FileSystem = fileSystem;
        }
    }
}
