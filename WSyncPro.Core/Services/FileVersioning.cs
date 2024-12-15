using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    public class FileVersioning : IFileVersioning
    {
        public Task<FileHistorySnapShot> CompareFile(WFile oldFile, WFile newFile, string? jobId)
        {
            try
            {
                if (oldFile == null || newFile == null)
                {
                    throw new ArgumentNullException(nameof(oldFile), "Old file or new file cannot be null.");
                }

                // Create the snapshot, comparing fields and preserving unchanged values
                var snapshot = new FileHistorySnapShot
                {
                    Id = Guid.NewGuid(), // Unique ID for the snapshot
                    TimeStamp = (oldFile.LastUpdated, newFile.LastUpdated),

                    Filesize = (
                        oldFile.FileSize == newFile.FileSize ? oldFile.FileSize : 0,
                        oldFile.FileSize == newFile.FileSize ? newFile.FileSize : newFile.FileSize
                    ),

                    Filename = (
                        oldFile.Name == newFile.Name ? oldFile.Name : null,
                        oldFile.Name == newFile.Name ? null : newFile.Name
                    ),

                    FilePath = (
                        oldFile.Path == newFile.Path ? oldFile.Path : null,
                        oldFile.Path == newFile.Path ? null : newFile.Path
                    ),

                    LastEdited = (
                        oldFile.LastUpdated.ToString("o"),
                        newFile.LastUpdated.ToString("o")
                    ),

                    TriggerJobId = (
                        jobId ?? "unknown",
                        jobId ?? "unknown"
                    )
                };

                return Task.FromResult(snapshot);
            }
            catch (ArgumentNullException ex)
            {
                // Log specific null argument exception
                Console.Error.WriteLine($"Argument null error: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
            catch (Exception ex)
            {
                // Log general errors
                Console.Error.WriteLine($"An error occurred while comparing files: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
        }


    }
}
