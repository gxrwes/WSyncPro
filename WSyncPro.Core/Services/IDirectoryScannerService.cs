using System.Collections.Generic;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;

namespace WSyncPro.Core.Services
{
    public interface IDirectoryScannerService
    {
        Task<List<WObject>> ScanAsync(SyncJob job);
    }
}
