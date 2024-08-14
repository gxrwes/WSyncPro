using System;
using System.Collections.Generic;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Services.ArchiveServices
{
    internal class ArchiveFunctionReplacer
    {
        public static string DirectoryStructureReplacer(string directoryStructure, Job job, WFile file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(directoryStructure))
                    throw new ArgumentNullException(nameof(directoryStructure), "Directory structure template cannot be null or empty.");

                var replacements = new Dictionary<DirectoryTypesArchive, Func<string>>
                {
                    { DirectoryTypesArchive.YEAR, () => DateTime.Now.Year.ToString() },
                    { DirectoryTypesArchive.MONTH, () => DateTime.Now.Month.ToString("D2") },
                    { DirectoryTypesArchive.DAY, () => DateTime.Now.Day.ToString("D2") },
                    { DirectoryTypesArchive.TIME, () => DateTime.Now.ToString("HHmmss") },
                    { DirectoryTypesArchive.JOBID, () =>
                        job != null && !string.IsNullOrWhiteSpace(job.Id)
                        ? job.Id
                        : throw new ArgumentException("Job ID is required but was not provided.") },
                    { DirectoryTypesArchive.ITEMID, () =>
                        file != null && file.id != 0
                        ? file.id.ToString()
                        : throw new ArgumentException("File ID is required but was not provided or is invalid.") },
                    { DirectoryTypesArchive.JOBNAME, () =>
                        job != null && !string.IsNullOrWhiteSpace(job.Name)
                        ? job.Name
                        : throw new ArgumentException("Job name is required but was not provided.") },
                    { DirectoryTypesArchive.FILENAME, () =>
                        file != null && !string.IsNullOrWhiteSpace(file.Name)
                        ? file.Name
                        : throw new ArgumentException("File name is required but was not provided.") },
                    { DirectoryTypesArchive.CUSTOM, () => "CustomValue" } // Replace with actual logic if needed
                };

                // Handle video-specific properties
                if (file is WVideoFile videoFile)
                {
                    replacements[DirectoryTypesArchive.RESOLUTION] = () =>
                        videoFile.Resolution != Resolution.CUSTOM
                        ? videoFile.Resolution.ToString()
                        : throw new ArgumentException("Video file resolution is set to CUSTOM, which is not allowed.");
                }

                // Perform replacements only if the placeholder exists in the directoryStructure string
                foreach (var replacement in replacements)
                {
                    string placeholder = replacement.Key.ToString();
                    if (directoryStructure.Contains(placeholder))
                    {
                        directoryStructure = directoryStructure.Replace(placeholder, replacement.Value());
                    }
                }

                return directoryStructure;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DirectoryStructureReplacer: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller if necessary
            }
        }
    }
}
