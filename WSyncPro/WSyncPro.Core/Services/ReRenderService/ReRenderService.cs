﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.State;

namespace WSyncPro.Core.Services.ReRenderService
{
    public class ReRenderService
    {
        public string HandBreakPostFix { get; set; } = "_HB";

        private readonly string _handBrakeCliPath;

        public ReRenderService()
        {
            _handBrakeCliPath = StateManager.Instance.HandBrakeCliPath;

            if (string.IsNullOrWhiteSpace(_handBrakeCliPath) || !File.Exists(_handBrakeCliPath))
            {
                throw new InvalidOperationException("HandBrakeCLI path is not set or is invalid.");
            }
        }

        public void ExecuteReRender(Job job, WDirectory directory = null)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            foreach (var file in directory.Files.OfType<WVideoFile>())
            {
                if (FileContainsPreset(file, job.ReRenderOptions.Preset.ToString())) continue;

                string outputFilePath = GenerateOutputFilePath(file, job.ReRenderOptions.Preset.ToString());
                ReRenderFile(file.Path, outputFilePath, job.ReRenderOptions.Preset.ToString(), job.ReRenderOptions.AdvancedOptions);
            }
        }


        private bool FileContainsPreset(WVideoFile file, string preset)
        {
            return file.Name.Contains(preset, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateOutputFilePath(WVideoFile file, string preset)
        {
            string directoryPath = Path.GetDirectoryName(file.Path);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Path);
            string extension = Path.GetExtension(file.Path);

            return Path.Combine(directoryPath, $"{fileNameWithoutExtension}{HandBreakPostFix}_{preset}{extension}");
        }

        private void ReRenderFile(string inputFilePath, string outputFilePath, string preset, string advancedOptions)
        {
            string arguments = $"-i \"{inputFilePath}\" -o \"{outputFilePath}\" --preset \"{preset}\" {advancedOptions}";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _handBrakeCliPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"HandBrakeCLI failed to re-render the file. Error: {process.StandardError.ReadToEnd()}");
                }
            }
        }
    }


}
