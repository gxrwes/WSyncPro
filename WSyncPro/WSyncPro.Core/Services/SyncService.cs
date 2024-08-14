using System;
using System.Collections.Generic;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ScanService;

namespace WSyncPro.Core.Services
{
    public class SyncService
    {
        private readonly DirectoryScanner _scanner;
        private readonly CopyService _copyService;

        public SyncService()
        {
            _scanner = new DirectoryScanner();
            _copyService = new CopyService();
        }
        public SyncService(DirectoryScanner scanner, CopyService copyService)
        {
            _scanner = scanner;
            _copyService = copyService;
        }

        public void ExecuteJobs(IEnumerable<Job> jobs, WDirectory providedDirectory = null)
        {
            foreach (var job in jobs)
            {
                if (!job.IsEnabled)
                {
                    Console.WriteLine($"Job '{job.Name}' is disabled and will be skipped.");
                    continue;
                }

                WDirectory sourceDirectory = providedDirectory;

                if (sourceDirectory == null)
                {
                    if (string.IsNullOrWhiteSpace(job.SourceDirectory))
                    {
                        throw new ArgumentException("SourceDirectory cannot be null or whitespace.", nameof(job.SourceDirectory));
                    }

                    Console.WriteLine($"Scanning source directory: {job.SourceDirectory}");
                    sourceDirectory = _scanner.ScanDirectory(job.SourceDirectory, job.TargetedFileTypes, job.FilterStrings);
                }

                Console.WriteLine($"Copying files for job: {job.Name}");
                _copyService.CopyFiles(sourceDirectory, job, FileOverwriteOptions.ALWAYS, true);
            }
        }
    }
}
