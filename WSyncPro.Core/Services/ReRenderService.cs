using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models.Config;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Util.Services;

namespace WSyncPro.Core.Services
{
    public class ReRenderService : IRerenderService
    {
        public event EventHandler<ReRenderProgressEventArgs> ProgressChanged;

        private readonly IDirectoryScannerService _directoryScannerService;
        private readonly WEnv _envCache;

        public ReRenderService(IDirectoryScannerService directoryScannerService, WEnv envCache)
        {
            _directoryScannerService = directoryScannerService ?? throw new ArgumentNullException(nameof(directoryScannerService));
            _envCache = envCache ?? throw new ArgumentNullException(nameof(envCache));
        }

        public async Task ReRender(RenderJob job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            if (string.IsNullOrWhiteSpace(job.SrcDirectory)) throw new ArgumentException("Source directory is invalid.", nameof(job.SrcDirectory));

            var handBrakePath = GetHandBrakeCLIPath();
            var objects = await _directoryScannerService.ScanAsync(job);
            var filesToProcess = objects.OfType<WFile>().Select(w => w.FullPath).ToList();

            int processedFiles = 0;
            int totalFiles = filesToProcess.Count;

            foreach (var file in filesToProcess)
            {
                processedFiles++;
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var outputFilePath = Path.Combine(job.DstDirectory, $"{fileNameWithoutExtension}{job.VideoType.FileExtension}");

                var arguments = $"-i \"{file}\" -o \"{outputFilePath}\" --encoder {job.VideoType.CodecName}";
                await ExecuteHandBrakeCLI(handBrakePath, arguments);

                if (job.DeleteOriginal && File.Exists(file)) File.Delete(file);

                ProgressChanged?.Invoke(this, new ReRenderProgressEventArgs
                {
                    ProcessedFiles = processedFiles,
                    TotalFiles = totalFiles,
                    Message = $"Processed file {processedFiles} of {totalFiles}: {file}"
                });
            }
        }

        private string GetHandBrakeCLIPath()
        {
            const string envVarName = "HANDBRAKE_CLI";
            return _envCache.EnvVars.TryGetValue(envVarName, out var path) ? path : throw new InvalidOperationException("HandBrakeCLI path not set.");
        }

        private async Task ExecuteHandBrakeCLI(string handBrakePath, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = handBrakePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"HandBrakeCLI failed with exit code {process.ExitCode}.");
                }
            }
        }
    }

    public class ReRenderProgressEventArgs : EventArgs
    {
        public int ProcessedFiles { get; set; }
        public int TotalFiles { get; set; }
        public string Message { get; set; }
    }
}
