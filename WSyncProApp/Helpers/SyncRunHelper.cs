using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WSyncProApp.Models;

namespace WSyncProApp.Helpers
{
    public static class SyncRunHelper
    {
        private static readonly string SyncRunsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SyncRuns");

        public static void SaveSyncRun(SyncRun syncRun)
        {
            if (!Directory.Exists(SyncRunsDirectory))
            {
                Directory.CreateDirectory(SyncRunsDirectory);
            }

            string fileName = $"{syncRun.TimestampStart:yyyyMMdd_HHmmss}_SyncRun.json";
            string filePath = Path.Combine(SyncRunsDirectory, fileName);
            string jsonData = JsonConvert.SerializeObject(syncRun, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        public static List<SyncRun> LoadSyncRuns()
        {
            var syncRuns = new List<SyncRun>();

            if (Directory.Exists(SyncRunsDirectory))
            {
                var files = Directory.GetFiles(SyncRunsDirectory, "*_SyncRun.json");
                foreach (var file in files)
                {
                    string jsonData = File.ReadAllText(file);
                    var syncRun = JsonConvert.DeserializeObject<SyncRun>(jsonData);
                    syncRuns.Add(syncRun);
                }
            }

            return syncRuns;
        }
    }
}
