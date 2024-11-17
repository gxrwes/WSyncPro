using System;
using System.Runtime.Serialization;
using WSyncPro.Models.Enum.ReRenderEnumCollection;

namespace WSyncPro.Models.Content
{
    [Serializable]
    public class RenderJob : SyncJob
    {
        public VideoType VideoType { get; set; }
        public bool IsRunning { get; set; } = false;
        public bool DeleteOriginal { get; set; } = true;

        // Parameterless constructor for serialization frameworks
        public RenderJob()
        {
            VideoType = VideoType.MP4; // Set a default value to avoid null
        }

        // Constructor to initialize VideoType explicitly
        public RenderJob(VideoType videoType)
        {
            VideoType = videoType ?? throw new ArgumentNullException(nameof(videoType), "VideoType cannot be null.");
        }
    }
}
