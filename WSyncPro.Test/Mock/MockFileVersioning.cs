using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Files;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Test.Mock
{
    public class MockFileVersioning : IFileVersioning
    {
        public Task<FileHistorySnapShot> CompareFile(WFile? oldFile, WFile newFile, string? jobId)
        {
            return Task.FromResult(new FileHistorySnapShot
            {
                Id = Guid.NewGuid(),
                TimeStamp = (DateTime.Now, DateTime.Now),
                Filesize = (newFile.FileSize, newFile.FileSize),
                Filename = (newFile.Name, newFile.Name),
                FilePath = (newFile.Path, newFile.Path),
                LastEdited = ("MockUser", "MockUser"),
                TriggerJobId = (jobId ?? "", jobId ?? "")
            });
        }

        public Task<List<FileHistorySnapShot>> GetVersionHistoryForFile(List<FileHistorySnapShot> files, WFile file)
        {
            return Task.FromResult(new List<FileHistorySnapShot>());
        }
    }

}
