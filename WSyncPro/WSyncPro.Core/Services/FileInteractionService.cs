using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WSyncPro.Core.Managers;
using WSyncPro.Models;
using WSyncPro.Models.Enums;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Services
{
    public class FileInteractionService
    {
        // Semaphore for general operations
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // Separate semaphore for TrashFolder operations
        private readonly SemaphoreSlim _trashFolderSemaphore = new SemaphoreSlim(1, 1);
        private string _trashFolder;

        private readonly AppStateManager _appStateManager = AppStateManager.Instance;

        /// <summary>
        /// Asynchronously gets the TrashFolder path.
        /// </summary>
        public async Task<string> GetTrashFolderAsync()
        {
            await _trashFolderSemaphore.WaitAsync();
            try
            {
                return _trashFolder;
            }
            finally
            {
                _trashFolderSemaphore.Release();
            }
        }

        /// <summary>
        /// Asynchronously sets the TrashFolder path.
        /// </summary>
        public async Task SetTrashFolderAsync(string value)
        {
            await _trashFolderSemaphore.WaitAsync();
            try
            {
                _trashFolder = value;
            }
            finally
            {
                _trashFolderSemaphore.Release();
            }
        }

        /// <summary>
        /// Copies a file and updates the job progress.
        /// </summary>
        public async Task CopyFileAsync(string srcFilePath, string dstFilePath, bool force = false, bool ifNewer = true, Job job = null)
        {
            if (force || (ifNewer && IsSourceNewer(srcFilePath, dstFilePath)))
            {
                var directory = Path.GetDirectoryName(dstFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (FileStream sourceStream = File.Open(srcFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream destinationStream = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Update job progress
                await ReportProgressAsync(job, srcFilePath, true);
            }
        }

        /// <summary>
        /// Copies multiple files and updates the job progress.
        /// </summary>
        public async Task CopyFilesAsync(List<string> srcFilePaths, string dstFolder, bool force = false, bool ifNewer = true, Job job = null)
        {
            if (job != null)
            {
                InitializeJobProgress(job, srcFilePaths.Count);
            }

            var tasks = srcFilePaths.Select(src =>
                CopyFileAsync(src, Path.Combine(dstFolder, Path.GetFileName(src)), force, ifNewer, job));
            await Task.WhenAll(tasks);

            if (job != null)
            {
                CompleteJob(job);
            }
        }

        /// <summary>
        /// Moves a file and updates the job progress.
        /// </summary>
        public async Task MoveFileAsync(string srcFilePath, string dstFilePath, bool force = false, bool ifNewer = true, Job job = null)
        {
            await CopyFileAsync(srcFilePath, dstFilePath, force, ifNewer, job);
            await DeleteFileAsync(srcFilePath, job);
        }

        /// <summary>
        /// Moves multiple files and updates the job progress.
        /// </summary>
        public async Task MoveFilesAsync(List<string> srcFilePaths, string dstFolder, bool force = false, bool ifNewer = true, Job job = null)
        {
            if (job != null)
            {
                InitializeJobProgress(job, srcFilePaths.Count);
            }

            var tasks = srcFilePaths.Select(src =>
                MoveFileAsync(src, Path.Combine(dstFolder, Path.GetFileName(src)), force, ifNewer, job));
            await Task.WhenAll(tasks);

            if (job != null)
            {
                CompleteJob(job);
            }
        }

        /// <summary>
        /// Deletes a file and updates the job progress.
        /// </summary>
        public async Task<bool> DeleteFileAsync(string srcFilePath, Job job = null)
        {
            try
            {
                await _semaphore.WaitAsync();
                string trashFolder = await GetTrashFolderAsync();
                string trashedFilePath = Path.Combine(trashFolder, Path.GetFileName(srcFilePath));

                if (HasEnoughSpace(trashedFilePath))
                {
                    var directory = Path.GetDirectoryName(trashedFilePath);
                    if (directory != null && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.Move(srcFilePath, trashedFilePath);
                    Console.WriteLine($"Moved file to trash: {trashedFilePath}");
                }
                else
                {
                    File.Delete(srcFilePath);
                    Console.WriteLine($"Deleted file: {srcFilePath}");
                }

                // Update job progress
                await ReportProgressAsync(job, srcFilePath, true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteFileAsync: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private bool IsSourceNewer(string srcFilePath, string dstFilePath)
        {
            if (!File.Exists(dstFilePath))
                return true;

            var srcInfo = new FileInfo(srcFilePath);
            var dstInfo = new FileInfo(dstFilePath);

            return srcInfo.LastWriteTime > dstInfo.LastWriteTime;
        }

        protected virtual bool HasEnoughSpace(string destinationPath)
        {
            var fileInfo = new FileInfo(destinationPath);
            var availableSpace = new DriveInfo(Path.GetPathRoot(destinationPath)).AvailableFreeSpace;
            return fileInfo.Length <= availableSpace;
        }

        /// <summary>
        /// Initializes the job's progress by setting the total number of files.
        /// </summary>
        private void InitializeJobProgress(Job job, int totalFiles)
        {
            var progress = new JobProgress
            {
                JobId = job.Id,
                JobName = job.Name,
                TotalFilesToProcess = totalFiles,
                StartTime = DateTime.UtcNow,
                Status = JobStatus.Running
            };

            job.AddProgress(progress);
            _appStateManager.UpdateJobAsync(job).Wait();
        }

        /// <summary>
        /// Marks the job as completed.
        /// </summary>
        private void CompleteJob(Job job)
        {
            var progress = job.ProgressHistory.Last();
            progress.Status = JobStatus.Completed;
            progress.LastUpdated = DateTime.UtcNow;
            _appStateManager.UpdateJobAsync(job).Wait();
        }

        /// <summary>
        /// Updates the job progress during file operations.
        /// </summary>
        private async Task ReportProgressAsync(Job job, string filePath, bool isSuccess)
        {
            if (job == null) return;

            var progress = job.ProgressHistory.LastOrDefault();
            if (progress != null)
            {
                if (isSuccess)
                {
                    progress.FilesProcessedSuccessfully++;
                }
                else
                {
                    progress.FailedFiles.Add(filePath);
                }

                progress.LastUpdated = DateTime.UtcNow;
                await _appStateManager.UpdateJobAsync(job);
            }
        }
    }
}
