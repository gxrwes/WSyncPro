using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ScanService;
using WSyncPro.Core.State;

namespace WSyncPro.Core.Services.CleanService
{
    public class CleanService
    {
        private readonly DirectoryScanner _directoryScanner;
        private readonly MoveService _moveService;
        private readonly StateManager _stateManager;

        public CleanService(DirectoryScanner directoryScanner, MoveService moveService)
        {
            _directoryScanner = directoryScanner ?? throw new ArgumentNullException(nameof(directoryScanner));
            _moveService = moveService ?? throw new ArgumentNullException(nameof(moveService));
            _stateManager = StateManager.Instance;
        }

        public void ExecuteClean(Job job, WDirectory directory = null)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            // Ensure TrashDirectory is set
            if (string.IsNullOrWhiteSpace(_stateManager.TrashDirectory))
                throw new InvalidOperationException("Trash directory is not set in the StateManager.");

            // Ensure TrashDirectory is a valid path
            if (!IsValidPath(_stateManager.TrashDirectory))
                throw new ArgumentException("The Trash directory path is invalid.", nameof(_stateManager.TrashDirectory));


            // Use the provided directory or scan the source directory
            directory ??= _directoryScanner.ScanDirectory(job.SourceDirectory, job.TargetedFileTypes, job.FilterStrings);

            // Move the files to the trash directory
            _moveService.MoveFiles(directory, new Job
            {
                TargetDirectory = _stateManager.TrashDirectory
            }, FileOverwriteOptions.ALWAYS, keepDirectoryStructure: true);
        }

        private bool IsValidPath(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
