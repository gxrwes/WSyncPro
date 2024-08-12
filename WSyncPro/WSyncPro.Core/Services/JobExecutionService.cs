using System;
using System.IO;
using WSyncPro.Core.Models;

namespace WSyncPro.Core.Services
{
    public class JobExecutionService
    {
        public SyncRun ExecuteJob(Job job)
        {
            var syncRun = new SyncRun
            {
                StartTime = DateTime.Now,
                RelatedJob = job
            };

            // Job execution logic based on JobType
            switch (job.JobType)
            {
                case JobType.Sync:
                    ExecuteSyncJob(job, syncRun);
                    break;
                case JobType.Archive:
                    ExecuteArchiveJob(job, syncRun);
                    break;
                case JobType.Clean:
                    ExecuteCleanJob(job, syncRun);
                    break;
                case JobType.ReRender:
                    ExecuteReRenderJob(job, syncRun);
                    break;
            }

            syncRun.EndTime = DateTime.Now;
            return syncRun;
        }

        private void ExecuteSyncJob(Job job, SyncRun syncRun)
        {
            // Sync job implementation: Copy files from source to target
            foreach (var file in Directory.GetFiles(job.SourceDirectory))
            {
                string targetFile = Path.Combine(job.TargetDirectory, Path.GetFileName(file));
                if (!File.Exists(targetFile) || File.GetLastWriteTime(targetFile) < File.GetLastWriteTime(file))
                {
                    File.Copy(file, targetFile, true);
                    syncRun.FilesTransferred++;
                    syncRun.Files.Add(new SyncFileMetadata
                    {
                        FileName = targetFile,
                        FileSize = new FileInfo(targetFile).Length,
                        LastModified = File.GetLastWriteTime(targetFile)
                    });
                }
            }
        }

        private void ExecuteArchiveJob(Job job, SyncRun syncRun)
        {
            // Archive job implementation: Group files into zips based on user options
            // Implementation goes here
        }

        private void ExecuteCleanJob(Job job, SyncRun syncRun)
        {
            // Clean job implementation: Move files to trash directory
            // Implementation goes here
        }

        private void ExecuteReRenderJob(Job job, SyncRun syncRun)
        {
            // ReRender job implementation: Use HandBrakeCLI to re-encode videos
            // Implementation goes here
        }
    }
}
