using System.Threading.Tasks;

namespace WSyncPro.Core.Services.VideoProbing
{
    /// <summary>
    /// Defines the parameters for converting a video.
    /// </summary>
    public class VideoConversionParameters
    {
        /// <summary>
        /// The desired output format (for example, "mp4", "avi", etc.).
        /// </summary>
        public string OutputFormat { get; set; } = "mp4";

        /// <summary>
        /// (Optional) The desired video codec.
        /// </summary>
        public string? VideoCodec { get; set; }

        /// <summary>
        /// (Optional) The desired audio codec.
        /// </summary>
        public string? AudioCodec { get; set; }

        /// <summary>
        /// (Optional) The desired video bitrate (for example, "800k").
        /// </summary>
        public string? VideoBitRate { get; set; }

        /// <summary>
        /// (Optional) The desired audio bitrate (for example, "128k").
        /// </summary>
        public string? AudioBitRate { get; set; }

        /// <summary>
        /// (Optional) The desired video frame rate.
        /// </summary>
        public double? FrameRate { get; set; }

        /// <summary>
        /// (Optional) The desired output video width.
        /// </summary>
        public int? TargetWidth { get; set; }

        /// <summary>
        /// (Optional) The desired output video height.
        /// </summary>
        public int? TargetHeight { get; set; }

        /// <summary>
        /// (Optional) Any additional custom FFmpeg arguments.
        /// </summary>
        public string? CustomFFMpegArgs { get; set; }
    }

    public interface IVideoConverter
    {
        /// <summary>
        /// Converts a single video file using the specified conversion parameters.
        /// </summary>
        /// <param name="filepath">The full path of the input video file.</param>
        /// <param name="parameters">Conversion parameters.</param>
        Task ConvertVideo(string filepath, VideoConversionParameters parameters);

        /// <summary>
        /// Converts all video files in the specified folder (and its subfolders) using the given parameters.
        /// </summary>
        /// <param name="folderPath">The folder to scan for video files.</param>
        /// <param name="parameters">Conversion parameters.</param>
        Task ConvertVideoFolder(string folderPath, VideoConversionParameters parameters);
    }
}
