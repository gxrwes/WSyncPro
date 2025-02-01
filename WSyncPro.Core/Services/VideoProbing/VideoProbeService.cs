using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NReco.VideoInfo;
using WSyncPro.Models.Files;

namespace WSyncPro.Core.Services.VideoProbing
{
    public class VideoProbeService : IVideoProbeService
    {
        public async Task RunInfoProbe(string folderPath)
        {
            // Define common video file extensions (adjust as needed)
            var videoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".mpeg", ".mpg"
            };

            // Recursively get all files in the folder with the specified extensions
            var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                 .Where(file => videoExtensions.Contains(Path.GetExtension(file)));

            // Create an instance of FFProbe (from NReco.VideoInfo)
            var ffProbe = new FFProbe();

            foreach (var file in files)
            {
                try
                {
                    // Retrieve media info for the video file
                    var mediaInfo = ffProbe.GetMediaInfo(file);
                    var fileInfo = new FileInfo(file);

                    // Select the first video and audio streams, if available
                    var videoStream = mediaInfo.Streams.FirstOrDefault(s => s.CodecType == "video");
                    var audioStream = mediaInfo.Streams.FirstOrDefault(s => s.CodecType == "audio");

                    // Since videoStream.FrameRate is already a float, simply use it directly
                    double frameRate = videoStream != null ? videoStream.FrameRate : 0;

                    // Compute an approximate overall bitrate using file size and duration
                    double overallBitRate = (mediaInfo.Duration.TotalSeconds > 0)
                        ? (fileInfo.Length * 8.0 / mediaInfo.Duration.TotalSeconds)
                        : 0;

                    // Map the extracted metadata to your FileInfoPayload model
                    var payload = new FileInfoPayload
                    {
                        FileName = Path.GetFileName(file),
                        FileSize = fileInfo.Length,
                        DurationSeconds = mediaInfo.Duration.TotalSeconds,
                        Width = videoStream?.Width ?? 0,
                        Height = videoStream?.Height ?? 0,
                        VideoCodec = videoStream?.CodecName ?? string.Empty,
                        AudioCodec = audioStream?.CodecName ?? string.Empty,
                        FrameRate = frameRate,
                        Format = mediaInfo.FormatName, // e.g., "mov,mp4,m4a,3gp,3g2,mj2"
                        BitRate = overallBitRate
                    };

                    // Serialize the metadata to JSON
                    var json = JsonConvert.SerializeObject(payload, Formatting.Indented);

                    // Save the JSON to a file next to the video with ".metainf" appended
                    var jsonFilePath = file + ".metainf";
                    await File.WriteAllTextAsync(jsonFilePath, json);

                    Console.WriteLine($"Processed '{file}' and created metadata file '{jsonFilePath}'.");
                }
                catch (Exception ex)
                {
                    // Log errors as needed
                    Console.WriteLine($"Error processing file '{file}': {ex.Message}");
                }
            }
        }
    }
}
