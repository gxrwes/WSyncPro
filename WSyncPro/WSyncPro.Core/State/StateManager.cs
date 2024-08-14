using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WSyncPro.Core.Models;

namespace WSyncPro.Core.State
{
    public sealed class StateManager
    {
        private static readonly Lazy<StateManager> lazy = new Lazy<StateManager>(() => new StateManager());

        public static StateManager Instance => lazy.Value;

        public List<Job> Jobs { get; private set; }
        public List<SyncRun> SyncRuns { get; private set; }
        public string StateFilePath { get; set; } = "AppState.json";

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

        public void AddJob(Job job)
        {
            Jobs.Add(job);
        }

        public void AddSyncRun(SyncRun syncRun)
        {
            SyncRuns.Add(syncRun);
        }

        public void RemoveJob(Job job)
        {
            Jobs.Remove(job);
        }

        // Other methods to manage state can be added here
    }

   
}
