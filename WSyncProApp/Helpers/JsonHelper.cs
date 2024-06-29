using System.IO;
using WSyncProApp.Models;
using Newtonsoft.Json;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Helpers
{
    public static class JsonHelper
    {
        public static VersionInfo LoadVersionInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} was not found.");
            }

            var jsonData = File.ReadAllText(filePath);
            var versionInfo = JsonConvert.DeserializeObject<VersionInfo>(jsonData);
            return versionInfo;
        }

        public static List<Job> LoadJobs(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} was not found.");
            }

            var jsonData = File.ReadAllText(filePath);
            var jobs = JsonConvert.DeserializeObject<List<Job>>(jsonData);
            return jobs;
        }
    }
}
