using System.Collections.Generic;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.Data;
using WSyncPro.Models.Reporting;

namespace WSyncPro.Core.Services
{
    public interface IImportService
    {
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        Task LoadProflilesFromFile(string joblistFilePath);
        Task SaveProfilesToFile(string joblistFilePath);

        Task<List<ImportProfile>> GetProfiles();
        Task AddProfile(ImportProfile profile);
        Task AddProfile(List<ImportProfile> profiles);
        Task RemoveProfile(ImportProfile profile);
        Task<SyncSummary> RunSelectedProfile();

    }
}