using System;
using System.Collections.Generic;
using System.IO;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Services.ScanService
{
    public class DirectoryScanner
    {
        public WDirectory ScanDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
            }

            // Create a representation for the current directory
            var directory = new WDirectory
            {
                Name = Path.GetFileName(path),
                Path = path
            };

            // Scan all files in the current directory
            foreach (var file in Directory.GetFiles(path))
            {
                var wfile = CreateWFile(file);
                if (wfile != null)
                {
                    directory.Files.Add(wfile);
                }
            }

            // Recursively scan all subdirectories
            foreach (var dir in Directory.GetDirectories(path))
            {
                var subDirectory = ScanDirectory(dir);
                directory.Files.Add(subDirectory);
            }

            return directory;
        }

        private WFile CreateWFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            var fileInfo = new FileInfo(filePath);
            WFile wfile = null;

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                    wfile = new WImageFile
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        FileSizeInMB = fileInfo.Length / (1024f * 1024f)
                    };
                    // Optionally, set resolution here
                    break;

                case ".mp3":
                case ".wav":
                case ".flac":
                    wfile = new WAudioFile
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        FileSizeInMB = fileInfo.Length / (1024f * 1024f)
                    };
                    // Optionally, set length here
                    break;

                case ".mp4":
                case ".avi":
                case ".mkv":
                case ".mov":
                    wfile = new WVideoFile
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        FileSizeInMB = fileInfo.Length / (1024f * 1024f)
                    };
                    // Optionally, set resolution and length here
                    break;

                case ".pdf":
                case ".docx":
                case ".txt":
                    wfile = new WDocumentFile
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        FileSizeInMB = fileInfo.Length / (1024f * 1024f)
                    };
                    // Optionally, set page count and format here
                    break;

                case ".exe":
                case ".msi":
                    wfile = new WProgramFile
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        FileSizeInMB = fileInfo.Length / (1024f * 1024f)
                    };
                    // Optionally, set version and vendor here
                    break;

                default:
                    // Handle other file types or skip
                    break;
            }

            return wfile;
        }
    }
}
