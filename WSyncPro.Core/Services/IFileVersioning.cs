using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Versioning;

namespace WSyncPro.Core.Services
{
    public interface IFileVersioning
    {
        public Task<FileHistorySnapShot> CompareFile(WFile? oldFile, WFile newFile, string? jobId);

    }
}
