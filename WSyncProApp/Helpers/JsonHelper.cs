using System.IO;
using WSyncProApp.Models;
using Newtonsoft.Json;

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
    }
}
