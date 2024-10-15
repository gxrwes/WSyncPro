// WFile.cs
using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Data
{
    public class WFile : WObject
    {
        public string Filetype { get; set; }
        public long Filesize { get; set; } // Use long for file size in bytes
        public FileSizeMultiplyer FilesizeMultiplyer { get; set; }

        // Add this property to store the relative path
        public string RelativePath { get; set; }
    }
}
