using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Files
{
    public class FileInfoPayload
    {
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; } // Total bytes
        public double DurationSeconds { get; set; } // Total length in seconds
        public int Width { get; set; } // Video width
        public int Height { get; set; } // Video height
        public string VideoCodec { get; set; } // Video codec
        public string AudioCodec { get; set; } // Audio codec
        public double FrameRate { get; set; } // Frames per second
        public string Format { get; set; } // Container format (e.g., mp4, mkv)
        public double BitRate { get; set; } // Overall bitrate in bits per second
    }
}
