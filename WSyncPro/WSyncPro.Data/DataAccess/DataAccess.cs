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

        public List<Job> LoadJobs()
        {
            if (!File.Exists(_filePath))
                return new List<Job>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Job>>(json);
        }

        public void SaveJobs(List<Job> jobs)
        {
            var json = JsonConvert.SerializeObject(jobs, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public List<SyncRun> LoadSyncRuns()
        {
            if (!File.Exists(_filePath))
                return new List<SyncRun>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<SyncRun>>(json);
        }

        public void SaveSyncRuns(List<SyncRun> syncRuns)
        {
            var json = JsonConvert.SerializeObject(syncRuns, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
