using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Services.ScanService
{
    public class DirectoryScanner
    {
        public WDirectory ScanDirectory(string path, string[] targetedFileTypes = null, string[] filterStrings = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

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
                try
                {
                    if (IsFileIncluded(file, targetedFileTypes, filterStrings))
                    {
                        var wfile = CreateWFile(file);
                        if (wfile != null)
                        {
                            directory.Files.Add(wfile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle it according to your needs
                    Console.WriteLine($"Error processing file '{file}': {ex.Message}");
                }
            }

            // Recursively scan all subdirectories
            foreach (var dir in Directory.GetDirectories(path))
            {
                try
                {
                    var subDirectory = ScanDirectory(dir, targetedFileTypes, filterStrings);
                    directory.Files.Add(subDirectory);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it according to your needs
                    Console.WriteLine($"Error processing directory '{dir}': {ex.Message}");
                }
            }

            return directory;
        }

        private bool IsFileIncluded(string filePath, string[] targetedFileTypes, string[] filterStrings)
        {
            var fileName = Path.GetFileName(filePath);

            // Check if the file type matches the targeted file types
            if (targetedFileTypes != null && targetedFileTypes.Length > 0)
            {
                var fileExtension = Path.GetExtension(filePath).ToLower();
                if (!targetedFileTypes.Contains(fileExtension))
                {
                    return false;
                }
            }

            // Check if the file name matches any of the filter strings
            if (filterStrings != null && filterStrings.Length > 0)
            {
                bool matchesFilter = filterStrings.Any(filter => fileName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
                if (!matchesFilter)
                {
                    return false;
                }
            }

            return true; // Include the file if it passes all filters
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
                case ".md":
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
                    // Log unsupported file type or handle as needed
                    Console.WriteLine($"Unsupported file type: {extension}");
                    break;
            }

            return wfile;
        }
    }
}
