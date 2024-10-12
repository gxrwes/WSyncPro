using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Managers
{
    public sealed class StateManager : IStateManager
    {
        public Dictionary<SyncJob, SyncJobState> JobStates { get; set; }

        // Path to the joblist.json file
        private readonly string JobListFilePath;

        public StateManager(string jobListFilePath)
        {
            JobStates = new Dictionary<SyncJob, SyncJobState>();
            JobListFilePath = jobListFilePath;
        }

        public async Task TryLoadJobsFromFile()
        {
            try
            {
                if (File.Exists(JobListFilePath))
                {
                    var fileContent = await File.ReadAllTextAsync(JobListFilePath);
                    var jobs = JsonSerializer.Deserialize<List<SyncJob>>(fileContent, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                    if (jobs != null)
                    {
                        foreach (var job in jobs)
                        {
                            if (!JobStates.ContainsKey(job))
                            {
                                JobStates[job] = new SyncJobState(); // Initialize each job with a default state
                            }
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

        public async Task TrySaveJobsToFile()
        {
            try
            {
                var jobs = new List<SyncJob>(JobStates.Keys);
                var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }, WriteIndented = true };
                var jsonContent = JsonSerializer.Serialize(jobs, options);

                var directory = Path.GetDirectoryName(JobListFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(JobListFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save jobs to file: {ex.Message}.");
            }
        }

        public async Task<Dictionary<SyncJob, SyncJobState>> GetAllJobStates()
        {
            return await Task.FromResult(JobStates);
        }

        public async Task<List<SyncJob>> GetAllJobs()
        {
            return await Task.FromResult(new List<SyncJob>(JobStates.Keys));
        }

        public async Task UpdateJob(SyncJob job)
        {
            if (JobStates.ContainsKey(job))
            {
                // Update the job in the dictionary
                var state = JobStates[job];
                JobStates.Remove(job);
                JobStates[job] = state;
                await TrySaveJobsToFile();
            }
            else
            {
                Console.WriteLine($"Job with Id {job.Id} not found.");
            }
        }

        public async Task AddJob(SyncJob job)
        {
            if (!JobStates.ContainsKey(job))
            {
                JobStates.Add(job, new SyncJobState());
                await TrySaveJobsToFile();
            }
            else
            {
                Console.WriteLine($"Job with Id {job.Id} already exists.");
            }
        }

        public async Task RemoveJob(SyncJob job)
        {
            if (JobStates.ContainsKey(job))
            {
                JobStates.Remove(job);
                await TrySaveJobsToFile();
            }
            else
            {
                Console.WriteLine($"Job with Id {job.Id} not found.");
            }
        }
    }
}
