using System;
using System.IO;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;

namespace WSyncPro.Core.Services
{
    public class MoveService
    {
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

            MoveDirectory(sourceDirectory, job.TargetDirectory, overwriteOption, keepDirectoryStructure);
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
                    MoveFile(file.Path, targetPath, overwriteOption);
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
