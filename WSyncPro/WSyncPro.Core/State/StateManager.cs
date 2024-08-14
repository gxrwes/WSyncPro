using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using WSyncPro.Core.Models;

namespace WSyncPro.Core.State
{
    public sealed class StateManager
    {
        private static readonly Lazy<StateManager> lazy = new Lazy<StateManager>(() => new StateManager());

        // NEEDED DIRECTORIES
        public string TrashDirectory = "";
        public string HandBrakeCliPath = "";
        private readonly List<string> _logEntries = new List<string>();
        private readonly object _logLock = new object();
        private readonly object _progressLock = new object();
        private const int MaxRetryCount = 5;
        private const int RetryDelayMilliseconds = 500;

        // Holds the newest progress from each service
        private readonly Dictionary<string, ProgressModel> _serviceProgress = new Dictionary<string, ProgressModel>();

        public static StateManager Instance => lazy.Value;

        public List<Job> Jobs { get; private set; }
        public List<SyncRun> SyncRuns { get; private set; }
        public string StateFilePath { get; set; } = "AppState.json";
        public string LogFilePath { get; set; } = "WSyncPro_Log.txt";

        private StateManager()
        {
            LoadState();
        }

        public void LoadState()
        {
            if (File.Exists(StateFilePath))
            {
                var json = File.ReadAllText(StateFilePath);
                var state = JsonConvert.DeserializeObject<AppState>(json);

                Jobs = state.Jobs ?? new List<Job>();
                SyncRuns = state.SyncRuns ?? new List<SyncRun>();
            }
            else
            {
                Jobs = new List<Job>();
                SyncRuns = new List<SyncRun>();
            }
        }

        public void SaveState()
        {
            var state = new AppState
            {
                Jobs = this.Jobs,
                SyncRuns = this.SyncRuns
            };

            var json = JsonConvert.SerializeObject(state, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(StateFilePath, json);
        }

        public void Log(string message)
        {
            lock (_logLock)
            {
                _logEntries.Add($"{DateTime.Now}: {message}");
                SaveLog();
            }
        }

        private void SaveLog()
        {
            int attempt = 0;
            bool success = false;

            while (attempt < MaxRetryCount && !success)
            {
                try
                {
                    File.AppendAllLines(LogFilePath, _logEntries);
                    _logEntries.Clear();
                    success = true;
                }
                catch (IOException)
                {
                    attempt++;
                    Thread.Sleep(RetryDelayMilliseconds);
                }
            }

            if (!success)
            {
                // If it fails after all retries, log internally without throwing an exception.
                Console.WriteLine($"Failed to write log after {MaxRetryCount} attempts.");
            }
        }

        public void RegisterService(string serviceName)
        {
            Log($"Service {serviceName} started.");
        }

        public void CompleteService(string serviceName)
        {
            Log($"Service {serviceName} completed.");
        }

        public void UpdateProgress(string serviceName, string status, int percentageComplete)
        {
            lock (_progressLock)
            {
                var progressModel = new ProgressModel(serviceName, status, percentageComplete);
                _serviceProgress[serviceName] = progressModel;

                Log($"{serviceName} progress: {status} ({percentageComplete}%)");
            }
        }

        public ProgressModel GetLatestProgress(string serviceName)
        {
            lock (_progressLock)
            {
                return _serviceProgress.ContainsKey(serviceName) ? _serviceProgress[serviceName] : null;
            }
        }
    }
}
