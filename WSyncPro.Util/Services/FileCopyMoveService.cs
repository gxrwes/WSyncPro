using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WSyncPro.Util.Services
{
    public class FileCopyMoveService : IFileCopyMoveService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task CopyFileAsync(string srcFilePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(srcFilePath) || string.IsNullOrEmpty(destinationPath))
                throw new ArgumentException("Source or destination path cannot be null or empty.");

            if (!File.Exists(srcFilePath))
                throw new FileNotFoundException("Source file not found.", srcFilePath);

            await _semaphore.WaitAsync();
            try
            {
                await Task.Run(() => File.Copy(srcFilePath, destinationPath, overwrite: true));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task MoveFileAsync(string srcFilePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(srcFilePath) || string.IsNullOrEmpty(destinationPath))
                throw new ArgumentException("Source or destination path cannot be null or empty.");

            if (!File.Exists(srcFilePath))
                throw new FileNotFoundException("Source file not found.", srcFilePath);

            await _semaphore.WaitAsync();
            try
            {
                await Task.Run(() => File.Move(srcFilePath, destinationPath));
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
