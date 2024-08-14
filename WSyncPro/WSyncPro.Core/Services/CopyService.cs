using System;
using System.IO;
using System.Linq;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.State;

namespace WSyncPro.Core.Services
{
    public class CopyService
    {
        private readonly StateManager _stateManager;

        public CopyService()
        {
            _stateManager = StateManager.Instance;
        }

        public void CopyFiles(WDirectory sourceDirectory, Job job, FileOverwriteOptions overwriteOption, bool keepDirectoryStructure)
        {
            if (sourceDirectory == null)
                throw new ArgumentNullException(nameof(sourceDirectory));

            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrWhiteSpace(job.TargetDirectory))
                throw new ArgumentException("Job.TargetDirectory cannot be null or whitespace.", nameof(job.TargetDirectory));

            if (!Directory.Exists(job.TargetDirectory))
            {
                Directory.CreateDirectory(job.TargetDirectory);
            }

            _stateManager.RegisterService("CopyService");

            try
            {
                CopyDirectory(sourceDirectory, job.TargetDirectory, overwriteOption, keepDirectoryStructure);
                _stateManager.CompleteService("CopyService");
            }
            catch (Exception ex)
            {
                _stateManager.Log($"Error in CopyService: {ex.Message}");
                throw;
            }
        }

        private void CopyDirectory(WDirectory sourceDirectory, string targetDirectory, FileOverwriteOptions overwriteOption, bool keepDirectoryStructure)
        {
            foreach (var file in sourceDirectory.Files)
            {
                string targetPath = keepDirectoryStructure
                    ? Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory.Path, file.Path))
                    : Path.Combine(targetDirectory, file.Name);

                if (file is WDirectory subDirectory)
                {
                    if (keepDirectoryStructure)
                    {
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                        CopyDirectory(subDirectory, targetPath, overwriteOption, keepDirectoryStructure);
                    }
                }
                else
                {
                    _stateManager.UpdateProgress("CopyService", $"Copying {file.Name}", 50);
                    CopyFile(file.Path, targetPath, overwriteOption);
                    _stateManager.UpdateProgress("CopyService", $"Copied {file.Name}", 100);
                }
            }
        }

        private void CopyFile(string sourcePath, string targetPath, FileOverwriteOptions overwriteOption)
        {
            if (File.Exists(targetPath))
            {
                bool shouldOverwrite = false;

                switch (overwriteOption)
                {
                    case FileOverwriteOptions.ALWAYS:
                        shouldOverwrite = true;
                        break;
                    case FileOverwriteOptions.NEVER:
                        shouldOverwrite = false;
                        break;
                    case FileOverwriteOptions.NEWER:
                        shouldOverwrite = File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath);
                        break;
                }

                if (!shouldOverwrite)
                {
                    return;
                }
            }

            File.Copy(sourcePath, targetPath, true);
        }
    }
}
