using System;
using System.IO;
using Newtonsoft.Json;
using WSyncProApp.Models;

namespace WSyncProApp.Helpers
{
    public static class SettingsHelper
    {
        private static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public static AppSettings LoadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return new AppSettings();
            }

            string jsonData = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(jsonData);
        }

        public static void SaveSettings(AppSettings settings)
        {
            string jsonData = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, jsonData);
        }
    }
}
