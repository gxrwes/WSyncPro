using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Files;
using WSyncPro.Models.Jobs;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    public class CopyService : ICopyService
    {
        private readonly IFileVersioning _fileVersioning;
        private readonly IAppCache _cache;
        private readonly ILogger<CopyService> _logger;

        public CopyService(IFileVersioning fileVersioning, IAppCache cache, ILogger<CopyService> logger)
        {
            _fileVersioning = fileVersioning;
            _cache = cache;
            _logger = logger;
        }

        public async Task CopyFile(CopyJob copyJob)
        {
            try
            {
                var snapshot = new FileHistorySnapShot();
                if (copyJob == null)
                {
                    throw new ArgumentNullException(nameof(copyJob), "Copy job cannot be null.");
                }


                // Ensure the destination directory exists
                var destinationDirectory = Path.GetDirectoryName(copyJob.DstFilePathAbsolute);
                if (string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    throw new ArgumentException("Destination directory is invalid.", nameof(copyJob.DstFilePathAbsolute));
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Check if the file already exists at the destination
                if (File.Exists(copyJob.DstFilePathAbsolute))
                {
                    _logger.LogInformation("File already exists at destination: {DestinationPath}", copyJob.DstFilePathAbsolute);

                    // Create FileHistorySnapShot for the existing file
                    var existingFileInfo = new FileInfo(copyJob.DstFilePathAbsolute);
                    var existingFile = new WFile
                    {
                        Path = existingFileInfo.FullName,
                        Name = existingFileInfo.Name,
                        FileSize = (int)existingFileInfo.Length,
                        LastUpdated = existingFileInfo.LastWriteTime,
                        Created = existingFileInfo.CreationTime,
                        FileExtension = existingFileInfo.Extension
                    };

                    var sourceFileInfo = new FileInfo(copyJob.SrcFilePathAbsolute);
                    if (!sourceFileInfo.Exists)
                    {
                        throw new FileNotFoundException("Source file does not exist.", copyJob.SrcFilePathAbsolute);
                    }

                    var sourceFile = new WFile
                    {
                        Path = sourceFileInfo.FullName,
                        Name = sourceFileInfo.Name,
                        FileSize = (int)sourceFileInfo.Length,
                        LastUpdated = sourceFileInfo.LastWriteTime,
                        Created = sourceFileInfo.CreationTime,
                        FileExtension = sourceFileInfo.Extension
                    };

                    snapshot = await _fileVersioning.CompareFile(existingFile, sourceFile, copyJob.Guid.ToString());
                    await _cache.AddFileHistorySnapshot(snapshot);
                }
                else
                {
                    var sourceFileInfo = new FileInfo(copyJob.SrcFilePathAbsolute);
                    if (!sourceFileInfo.Exists)
                    {
                        throw new FileNotFoundException("Source file does not exist.", copyJob.SrcFilePathAbsolute);
                    }

                    var sourceFile = new WFile
                    {
                        Path = sourceFileInfo.FullName,
                        Name = sourceFileInfo.Name,
                        FileSize = (int)sourceFileInfo.Length,
                        LastUpdated = sourceFileInfo.LastWriteTime,
                        Created = sourceFileInfo.CreationTime,
                        FileExtension = sourceFileInfo.Extension
                    };
                    snapshot = await _fileVersioning.CompareFile(null, sourceFile, copyJob.Guid.ToString());
                    await _cache.AddFileHistorySnapshot(snapshot);
                }

                // Perform the copy operation
                if (!File.Exists(copyJob.SrcFilePathAbsolute))
                {
                    throw new FileNotFoundException("Source file does not exist.", copyJob.SrcFilePathAbsolute);
                }

                using (var sourceStream = new FileStream(copyJob.SrcFilePathAbsolute, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var destinationStream = new FileStream(copyJob.DstFilePathAbsolute, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                _logger.LogInformation("File copied successfully from {SourcePath} to {DestinationPath}", copyJob.SrcFilePathAbsolute, copyJob.DstFilePathAbsolute);
                copyJob.Successful = true;
                
                // Hook versioning
                var snapshotList = new List<FileHistorySnapShot>();
                snapshotList.Add(snapshot);
                var jobExcecution = new JobExecution(copyJob.Guid.ToString(), JobStatus.Successful, snapshotList);
                await _cache.AddJobExecution(jobExcecution);
            }
            catch (Exception ex)
            {
                var jobExcecution = new JobExecution(copyJob.Guid.ToString(), JobStatus.Failed, new List<FileHistorySnapShot>());
                _logger.LogError(ex, "Error copying file from {SourcePath} to {DestinationPath}", copyJob.SrcFilePathAbsolute, copyJob.DstFilePathAbsolute);
                throw; // Preserve the stack trace
            }
        }


        public async Task MoveFile(CopyJob moveJob)
        {
            try
            {
                if (moveJob == null)
                {
                    throw new ArgumentNullException(nameof(moveJob), "Move job cannot be null.");
                }

                // Ensure the destination directory exists
                var destinationDirectory = Path.GetDirectoryName(moveJob.DstFilePathAbsolute);
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Check if the file already exists at the destination
                if (File.Exists(moveJob.DstFilePathAbsolute))
                {
                    _logger.LogInformation("File already exists at destination: {DestinationPath}", moveJob.DstFilePathAbsolute);

                    // Create FileHistorySnapShot for the existing file
                    var existingFileInfo = new FileInfo(moveJob.DstFilePathAbsolute);
                    var existingFile = new WFile
                    {
                        Path = existingFileInfo.FullName,
                        Name = existingFileInfo.Name,
                        FileSize = (int)existingFileInfo.Length,
                        LastUpdated = existingFileInfo.LastWriteTime,
                        Created = existingFileInfo.CreationTime,
                        FileExtension = existingFileInfo.Extension
                    };

                    var sourceFileInfo = new FileInfo(moveJob.SrcFilePathAbsolute);
                    var sourceFile = new WFile
                    {
                        Path = sourceFileInfo.FullName,
                        Name = sourceFileInfo.Name,
                        FileSize = (int)sourceFileInfo.Length,
                        LastUpdated = sourceFileInfo.LastWriteTime,
                        Created = sourceFileInfo.CreationTime,
                        FileExtension = sourceFileInfo.Extension
                    };

                    var snapshot = await _fileVersioning.CompareFile(existingFile, sourceFile, moveJob.Guid.ToString());
                    await _cache.AddFileHistorySnapshot(snapshot);
                }

                // Perform the move operation
                File.Move(moveJob.SrcFilePathAbsolute, moveJob.DstFilePathAbsolute, overwrite: true);

                _logger.LogInformation("File moved successfully from {SourcePath} to {DestinationPath}", moveJob.SrcFilePathAbsolute, moveJob.DstFilePathAbsolute);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving file from {SourcePath} to {DestinationPath}", moveJob.SrcFilePathAbsolute, moveJob.DstFilePathAbsolute);
                throw;
            }
        }
    }
}
