using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WSyncPro.Core.Models;

namespace WSyncPro.Data.DataAccess
{
    public class JsonDataAccess
    {
        private readonly string _filePath;

        public JsonDataAccess(string filePath)
        {
            _filePath = filePath;
        }

        // Load Jobs from JSON file
        public List<Job> LoadJobs()
        {
            if (!File.Exists(_filePath))
                return new List<Job>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Job>>(json);
        }

        // Save Jobs to JSON file
        public void SaveJobs(List<Job> jobs)
        {
            var json = JsonConvert.SerializeObject(jobs, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Load SyncRuns from JSON file
        public List<SyncRun> LoadSyncRuns()
        {
            if (!File.Exists(_filePath))
                return new List<SyncRun>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<SyncRun>>(json);
        }

        // Save SyncRuns to JSON file
        public void SaveSyncRuns(List<SyncRun> syncRuns)
        {
            var json = JsonConvert.SerializeObject(syncRuns, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Load Application Settings from JSON file
        public ApplicationSettings LoadSettings()
        {
            if (!File.Exists(_filePath))
                return new ApplicationSettings();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        // Save Application Settings to JSON file
        public void SaveSettings(ApplicationSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
