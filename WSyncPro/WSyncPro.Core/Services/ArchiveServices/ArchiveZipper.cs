using System;
using System.IO;
using System.IO.Compression;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Services.ArchiveServices
{
    public class ArchiveZiper
    {
        public void CreateZipArchive(WDirectory directory, string targetDirectory, string zipFileName, bool overwrite = false)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                throw new ArgumentException("Target directory cannot be null or whitespace.", nameof(targetDirectory));
            }

            if (string.IsNullOrWhiteSpace(zipFileName))
            {
                throw new ArgumentException("Zip file name cannot be null or whitespace.", nameof(zipFileName));
            }

            // Ensure the target directory exists
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string zipFilePath = Path.Combine(targetDirectory, zipFileName);

            // Check if the file exists
            if (File.Exists(zipFilePath))
            {
                if (overwrite)
                {
                    // Delete the existing file if overwrite is true
                    File.Delete(zipFilePath);
                }
                else
                {
                    throw new IOException($"The file '{zipFilePath}' already exists. Set overwrite to true if you want to overwrite it.");
                }
            }

            // Create a new zip archive at the specified location
            using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                AddDirectoryToZip(zipArchive, directory, directory.Path);
            }
        }

        private void AddDirectoryToZip(ZipArchive zipArchive, WDirectory directory, string baseDirectoryPath)
        {
            foreach (var file in directory.Files)
            {
                string relativePath = Path.GetRelativePath(baseDirectoryPath, file.Path);

                if (file is WDirectory subDirectory)
                {
                    AddDirectoryToZip(zipArchive, subDirectory, baseDirectoryPath);
                }
                else
                {
                    zipArchive.CreateEntryFromFile(file.Path, relativePath);
                }
            }
        }
    }
}
