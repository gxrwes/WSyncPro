using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Reporting;
using WSyncPro.Util.Files;
using WSyncPro.Util.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace WSyncPro
{
    public class ImportService : IImportService
    {
        private const string PROFILE_FILE = "./importprofiles.json";
        private readonly IFileLoader _fileLoader;
        private readonly IDirectoryScannerService _directoryScannerService;
        private readonly IFileCopyMoveService _fileCopyMoveService;
        private readonly ILogger<ImportService> _logger;
        private List<ImportProfile> _profiles = new List<ImportProfile>();

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public ImportService(
             IFileLoader fileLoader,
             IDirectoryScannerService directoryScannerService,
             IFileCopyMoveService fileCopyMoveService,
             ILogger<ImportService> logger)
        {
            _fileLoader = fileLoader;
            _directoryScannerService = directoryScannerService;
            _fileCopyMoveService = fileCopyMoveService;
            _logger = logger;
        }
        public Task AddProfile(ImportProfile profile)
        {
            if (_profiles.Any(p => p.Id == profile.Id))
            {
                var existingProfile = _profiles.First(p => p.Id == profile.Id);
                existingProfile.Name = profile.Name;
                existingProfile.Status = profile.Status;
                existingProfile.SubPathBuilder = profile.SubPathBuilder;
                existingProfile.DstDirectory = profile.DstDirectory;
                existingProfile.FilterExclude = profile.FilterExclude;
                existingProfile.FilterExclude = profile.FilterExclude;
                existingProfile.Enabled = profile.Enabled;
                _logger.LogInformation($"Updated existing profile: {profile.Name}");
            }
            else
            {
                _profiles.Add(profile);
                _logger.LogInformation($"Added new job: {profile.Name}");
            }
            return Task.CompletedTask;
        }

        public Task AddProfile(List<ImportProfile> profiles)
        {
            foreach (var profile in profiles)
            {
                AddProfile(profile);
            }
            return Task.CompletedTask;
        }

        public Task<List<ImportProfile>> GetProfiles()
        {
            _logger.LogDebug("Retrieved all profiles");
            return Task.FromResult(_profiles);
        }

        public async Task LoadProflilesFromFile(string profliesFilePath)
        {
            _logger.LogInformation($"Loading jobs from file: {profliesFilePath}");
            var loadedProfiles = await _fileLoader.LoadFileAndParseAsync<List<ImportProfile>>(profliesFilePath);
            _profiles = loadedProfiles ?? new List<ImportProfile>();
            _logger.LogInformation($"Loaded {_profiles.Count} profiles from file");

        }

        public Task RemoveProfile(ImportProfile profile)
        {
            _profiles.Remove(profile);
            return Task.CompletedTask;
        }

        public async Task SaveProfilesToFile(string profileFilePath)
        {
            _logger.LogInformation($"Saving jobs to file: {profileFilePath}");
            //await _fileLoader.SaveToFileAsObjectAsync(profileFilePath, ImportProfile);
            //_logger.LogInformation($"Saved {_profiles.Count} jobs to file");

        }

        public async Task<SyncSummary> RunSelectedProfile(Guid profileId, string importPath)
        {
            var importSummary = new SyncSummary();
            var stopwatch = Stopwatch.StartNew();

            var selectedProfile = _profiles.Where(p => p.Id == profileId).FirstOrDefault();
            try
            {
                if (selectedProfile != null)
                {
                    var objectsToSync = await _directoryScannerService.ScanAsync(selectedProfile, importPath);

                }




            }
            catch (Exception ex)
            {
            }
            throw new NotImplementedException();
        }

        public Task<SyncSummary> RunSelectedProfile()
        {
            throw new NotImplementedException();
        }
    }
}
