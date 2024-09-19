using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WSyncPro.Core.Services
{
    public class FileInteractionService
    {
        // Semaphore for general operations
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // Separate semaphore for TrashFolder operations
        private readonly SemaphoreSlim _trashFolderSemaphore = new SemaphoreSlim(1, 1);
        private string _trashFolder;

        /// <summary>
        /// Asynchronously gets the TrashFolder path.
        /// </summary>
        /// <returns>The current TrashFolder path.</returns>
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
        /// <param name="value">The new TrashFolder path.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        public async Task CopyFileAsync(string srcFilePath, string dstFilePath, bool force = false, bool ifNewer = true)
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
            }
        }

        public async Task CopyFilesAsync(List<string> srcFilePaths, string dstFolder, bool force = false, bool ifNewer = true)
        {
            var tasks = srcFilePaths.Select(src =>
                CopyFileAsync(src, Path.Combine(dstFolder, Path.GetFileName(src)), force, ifNewer));
            await Task.WhenAll(tasks);
        }

        public async Task MoveFileAsync(string srcFilePath, string dstFilePath, bool force = false, bool ifNewer = true)
        {
            await CopyFileAsync(srcFilePath, dstFilePath, force, ifNewer);
            await DeleteFileAsync(srcFilePath);
        }

        public async Task MoveFilesAsync(List<string> srcFilePaths, string dstFolder, bool force = false, bool ifNewer = true)
        {
            var tasks = srcFilePaths.Select(src =>
                MoveFileAsync(src, Path.Combine(dstFolder, Path.GetFileName(src)), force, ifNewer));
            await Task.WhenAll(tasks);
        }

        public async Task<bool> DeleteFileAsync(string srcFilePath)
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

        // Changed from private to protected virtual to allow overriding in tests
        protected virtual bool HasEnoughSpace(string destinationPath)
        {
            // Implement logic to check if there's enough space in the trash folder.
            // This could involve checking available disk space against the file size.
            var fileInfo = new FileInfo(destinationPath);
            var availableSpace = new DriveInfo(Path.GetPathRoot(destinationPath)).AvailableFreeSpace;
            return fileInfo.Length <= availableSpace;
        }
    }
}
