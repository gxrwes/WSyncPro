using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;
using WSyncPro.Util.Files;

namespace WSyncPro.Core.Managers
{
    public sealed class StateManager
    {
        private static readonly Lazy<StateManager> lazy = new Lazy<StateManager>(() => new StateManager());
        public static StateManager Instance => lazy.Value;

        public Dictionary<SyncJob, SyncJobState> JobStates { get; set; }

        // Path to the joblist.json file
        private const string JobListFilePath = "wwwroot/jobs/joblist.json";

        private StateManager()
        {
            JobStates = new Dictionary<SyncJob, SyncJobState>();

            // Load jobs from the file on first initialization
            TryLoadJobsFromFile();
        }

        private void TryLoadJobsFromFile()
        {
            try
            {
                if (File.Exists(JobListFilePath))
                {
                    var fileContent = File.ReadAllText(JobListFilePath);
                    var jobs = JsonSerializer.Deserialize<List<SyncJob>>(fileContent, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                    if (jobs != null)
                    {
                        foreach (var job in jobs)
                        {
                            JobStates[job] = new SyncJobState(); // Initialize each job with a default state
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Job list file not found at path: {JobListFilePath}. Initializing with an empty job list.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load jobs from file: {ex.Message}. Initializing with an empty job list.");
                JobStates = new Dictionary<SyncJob, SyncJobState>();
            }
        }
    }
}
