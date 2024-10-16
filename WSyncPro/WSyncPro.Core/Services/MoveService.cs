﻿using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.State;

namespace WSyncPro.Core.Services
{
    public class MoveService
    {
        private readonly StateManager _stateManager;

        public MoveService()
        {
            _stateManager = StateManager.Instance;
        }

        public void MoveFiles(WDirectory sourceDirectory, Job job, FileOverwriteOptions overwriteOption, bool keepDirectoryStructure)
        {
            if (sourceDirectory == null)
                throw new ArgumentNullException(nameof(sourceDirectory));

            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrWhiteSpace(job.TargetDirectory) || !IsValidPath(job.TargetDirectory))
                throw new ArgumentException("Job.TargetDirectory is invalid.", nameof(job.TargetDirectory));

            if (!Directory.Exists(job.TargetDirectory))
            {
                try
                {
                    Directory.CreateDirectory(job.TargetDirectory);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to create directory '{job.TargetDirectory}': {ex.Message}");
                }
            }

            _stateManager.RegisterService("MoveService");

            try
            {
                MoveDirectory(sourceDirectory, job.TargetDirectory, overwriteOption, keepDirectoryStructure);
                _stateManager.CompleteService("MoveService");
            }
            catch (Exception ex)
            {
                _stateManager.Log($"Error in MoveService: {ex.Message}");
                throw;
            }
        }

        private bool IsValidPath(string path)
        {
            try
            {
                string fullPath = Path.GetFullPath(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void MoveDirectory(WDirectory sourceDirectory, string targetDirectory, FileOverwriteOptions overwriteOption, bool keepDirectoryStructure)
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
                        MoveDirectory(subDirectory, targetPath, overwriteOption, keepDirectoryStructure);
                    }
                }
                else
                {
                    _stateManager.UpdateProgress("MoveService", $"Moving {file.Name}", 50);
                    MoveFile(file.Path, targetPath, overwriteOption);
                    _stateManager.UpdateProgress("MoveService", $"Moved {file.Name}", 100);
                }
            }
        }

        private void MoveFile(string sourcePath, string targetPath, FileOverwriteOptions overwriteOption)
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

                // Delete the target file if overwriting
                File.Delete(targetPath);
            }

            File.Move(sourcePath, targetPath);
        }
    }
}
