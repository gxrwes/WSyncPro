using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NReco.VideoConverter;

namespace WSyncPro.Core.Services.VideoProbing
{
    public class VideoConverter : IVideoConverter
    {
        private readonly FFMpegConverter _ffMpegConverter;

        public VideoConverter()
        {
            _ffMpegConverter = new FFMpegConverter();
        }

        public async Task ConvertVideo(string filepath, VideoConversionParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
                throw new ArgumentException("File path is invalid.", nameof(filepath));

            // Build the output file name.
            var directory = Path.GetDirectoryName(filepath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filepath);
            var outputFile = Path.Combine(directory, $"{fileNameWithoutExtension}_converted.{parameters.OutputFormat}");

            // Build conversion settings.
            var settings = new ConvertSettings();

            // Set video codec if provided.
            if (!string.IsNullOrEmpty(parameters.VideoCodec))
                settings.VideoCodec = parameters.VideoCodec;

            // Set audio codec if provided.
            if (!string.IsNullOrEmpty(parameters.AudioCodec))
                settings.AudioCodec = parameters.AudioCodec;

            // Prepare CustomOutputArgs for settings.
            string customArgs = string.Empty;

            // Set video bitrate if provided.
            if (!string.IsNullOrEmpty(parameters.VideoBitRate))
                customArgs += $" -b:v {parameters.VideoBitRate}";

            // Set audio bitrate if provided.
            if (!string.IsNullOrEmpty(parameters.AudioBitRate))
                customArgs += $" -b:a {parameters.AudioBitRate}";

            // Set target resolution if provided.
            if (parameters.TargetWidth.HasValue && parameters.TargetHeight.HasValue)
                customArgs += $" -s {parameters.TargetWidth.Value}x{parameters.TargetHeight.Value}";

            // Set frame rate if provided.
            if (parameters.FrameRate.HasValue)
                settings.VideoFrameRate = (int)Math.Round(parameters.FrameRate.Value);

            // Append any additional custom FFmpeg arguments.
            if (!string.IsNullOrEmpty(parameters.CustomFFMpegArgs))
                customArgs += " " + parameters.CustomFFMpegArgs;

            // Set the built custom arguments on the settings.
            if (!string.IsNullOrWhiteSpace(customArgs))
                settings.CustomOutputArgs = customArgs;

            // Run the conversion in a background task (since ConvertMedia is synchronous).
            await Task.Run(() =>
            {
                _ffMpegConverter.ConvertMedia(filepath, null, outputFile, parameters.OutputFormat, settings);
            });
        }

        public async Task ConvertVideoFolder(string folderPath, VideoConversionParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                throw new ArgumentException("Folder path is invalid.", nameof(folderPath));

            // Define allowed video file extensions.
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".mpeg", ".mpg"
            };

            // Get all video files in the folder (including subfolders).
            var videoFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                      .Where(f => allowedExtensions.Contains(Path.GetExtension(f)));

            // Process each file.
            foreach (var file in videoFiles)
            {
                try
                {
                    await ConvertVideo(file, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting file '{file}': {ex.Message}");
                }
            }
        }
    }
}
