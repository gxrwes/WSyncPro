using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    public class FileVersioning : IFileVersioning
    {
        public Task<FileHistorySnapShot> CompareFile(WFile? oldFile, WFile newFile, string? jobId)
        {
            try
            {
                if (newFile == null)
                {
                    throw new ArgumentNullException(nameof(newFile), "New file cannot be null.");
                }

                // Create the snapshot, comparing fields and preserving unchanged values
                var snapshot = new FileHistorySnapShot
                {
                    Id = Guid.NewGuid(), // Unique ID for the snapshot

                    TimeStamp = (
                        oldFile?.LastUpdated ?? DateTime.MinValue, // Default to DateTime.MinValue if oldFile is null
                        newFile.LastUpdated
                    ),

                    Filesize = (
                        oldFile?.FileSize ?? 0, // Default to 0 if oldFile is null
                        newFile.FileSize
                    ),

                    Filename = (
                        oldFile?.Name ?? "New File", // Default to "New File" if oldFile is null
                        newFile.Name
                    ),

                    FilePath = (
                        oldFile?.Path ?? "Unknown Path", // Default to "Unknown Path" if oldFile is null
                        newFile.Path
                    ),

                    LastEdited = (
                        oldFile?.LastUpdated.ToString("o") ?? "N/A", // Default to "N/A" if oldFile is null
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

        public Task<List<FileHistorySnapShot>> GetVersionHistoryForFile(List<FileHistorySnapShot> snapshots, WFile file)
        {
            try
            {
                if (snapshots == null || snapshots.Count == 0)
                {
                    throw new ArgumentException("Snapshots list cannot be null or empty.", nameof(snapshots));
                }

                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file), "File cannot be null.");
                }

                // Filter snapshots matching the file by Filename and FilePath
                var matchingSnapshots = snapshots
                    .Where(snapshot =>
                        snapshot.Filename.Item2 == file.Name &&
                        snapshot.FilePath.Item2 == file.Path)
                    .OrderBy(snapshot => snapshot.TimeStamp.Item2) // Sort by the "After" timestamp
                    .ToList();

                return Task.FromResult(matchingSnapshots);
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
                Console.Error.WriteLine($"An error occurred while retrieving version history: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
        }
    }
}
