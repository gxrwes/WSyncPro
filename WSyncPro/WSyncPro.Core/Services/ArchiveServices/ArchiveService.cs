using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ArchiveServices;
using WSyncPro.Core.Services.ScanService;
using WSyncPro.Core.State;

namespace WSyncPro.Core.Services
{
    public class ArchiveService
    {
        private readonly DirectoryScanner _directoryScanner;
        private readonly ArchiveZiper _archiveZiper;
        private readonly StateManager _stateManager;

        public ArchiveService(DirectoryScanner directoryScanner, ArchiveZiper archiveZiper)
        {
            _directoryScanner = directoryScanner ?? throw new ArgumentNullException(nameof(directoryScanner));
            _archiveZiper = archiveZiper ?? throw new ArgumentNullException(nameof(archiveZiper));
            _stateManager = StateManager.Instance;
        }

        public void ExecuteArchive(Job job, WDirectory directory = null)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            _stateManager.RegisterService("ArchiveService");

            try
            {
                // Use the provided directory or scan the source directory
                directory ??= _directoryScanner.ScanDirectory(job.SourceDirectory, job.TargetedFileTypes, job.FilterStrings);

                // Replace placeholders in the directory structure pattern
                string targetPath = ArchiveFunctionReplacer.DirectoryStructureReplacer(job.ArchiveOptions.GroupingPattern, job, directory);

                // Combine with the job's target directory
                string fullTargetPath = Path.Combine(job.TargetDirectory, targetPath);

                // Ensure target path exists
                if (!Directory.Exists(fullTargetPath))
                {
                    Directory.CreateDirectory(fullTargetPath);
                }

                // Zip each file in the provided directory
                foreach (var file in directory.Files)
                {
                    if (file is WDirectory subDirectory)
                    {
                        // Recursively archive subdirectories
                        ExecuteArchive(job, subDirectory);
                    }
                    else
                    {
                        string zipFileName = $"{directory.Name}.zip"; // Ensure the directory name is used for zip file
                        string zipFilePath = Path.Combine(fullTargetPath, zipFileName);

                        // Report progress
                        _stateManager.UpdateProgress("ArchiveService", $"Archiving {zipFilePath}", 50);

                        // Create the zip file for the directory
                        _archiveZiper.CreateZipArchive(directory, fullTargetPath, zipFileName, overwrite: true);

                        // Debugging output
                        Console.WriteLine($"Created zip file at {zipFilePath}");

                        _stateManager.UpdateProgress("ArchiveService", $"Archived {zipFilePath}", 100);
                    }
                }

                _stateManager.CompleteService("ArchiveService");
            }
            catch (Exception ex)
            {
                _stateManager.Log($"Error in ArchiveService: {ex.Message}");
                throw;
            }
        }
    }
}
