using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WSyncPro.Core.Services;
using WSyncPro.Models;
using WSyncPro.Models.Enums;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Managers
{
    public sealed class AppStateManager
    {
        private static readonly Lazy<AppStateManager> lazy = new Lazy<AppStateManager>(() => new AppStateManager());

        public static AppStateManager Instance => lazy.Value;

        public List<Job> Jobs { get; private set; }

        private AppStateSnapShot _currentAppState;

        private readonly string _appStateFilePath;
        private readonly string _logFilePath;
        private readonly FileSerialisationServiceJson _serializationService;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // List for in-memory logging
        private List<string> _logEntries = new List<string>();

        private AppStateManager()
        {
            Jobs = new List<Job>();

            string appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WSyncPro");

            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }

            _appStateFilePath = Path.Combine(appDataDirectory, "AppState.json");
            _logFilePath = Path.Combine(appDataDirectory, "SyncLog.txt");

            _serializationService = new FileSerialisationServiceJson();

            LoadAppStateAsync().Wait();
        }

        public async Task UpdateAppStateAsync()
        {
            var newAppState = new AppStateSnapShot
            {
                Guid = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Jobs = Jobs.ToList()
            };

            foreach (var job in Jobs)
            {
                if (!newAppState.JobStateList.ContainsKey(job.Id))
                {
                    newAppState.JobStateList[job.Id] = new List<JobProgress>();
                }

                newAppState.JobStateList[job.Id] = job.ProgressHistory.ToList();
            }

            if (!IsAppStateEqual(_currentAppState, newAppState))
            {
                await _semaphore.WaitAsync();
                try
                {
                    _currentAppState = newAppState;
                    await SaveAppStateAsync();
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public AppStateSnapShot GetAppState() => _currentAppState;

        public async Task UpdateJobAsync(Job job)
        {
            await _semaphore.WaitAsync();
            try
            {
                var existingJob = Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (existingJob != null)
                {
                    var index = Jobs.IndexOf(existingJob);
                    Jobs[index] = job;
                }
                else
                {
                    Jobs.Add(job);
                }

                await UpdateAppStateAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SaveAppStateAsync()
        {
            try
            {
                bool saveResult = await _serializationService.SaveClassToFileAsync(_appStateFilePath, _currentAppState);
                if (saveResult)
                {
                    Console.WriteLine("App state saved successfully.");
                }
                else
                {
                    Console.WriteLine("App state save failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving app state: {ex.Message}");
            }
        }

        public async Task LoadAppStateAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (File.Exists(_appStateFilePath))
                {
                    _currentAppState = await _serializationService.GetFileAsClassAsync<AppStateSnapShot>(_appStateFilePath);
                    Console.WriteLine("App state loaded successfully.");
                }
                else
                {
                    _currentAppState = new AppStateSnapShot
                    {
                        Guid = Guid.NewGuid(),
                        TimeStamp = DateTime.UtcNow
                    };
                    Console.WriteLine("No existing app state found. Initialized a new state.");
                }

                Jobs = _currentAppState?.Jobs?.ToList() ?? new List<Job>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading app state: {ex.Message}");
                _currentAppState = new AppStateSnapShot
                {
                    Guid = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow
                };
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void LogMessage(string message)
        {
            var timestampedMessage = $"{DateTime.UtcNow}: {message}";
            _logEntries.Add(timestampedMessage);

            // Optionally write to a log file as well
            try
            {
                File.AppendAllLines(_logFilePath, new[] { timestampedMessage });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log entry to file: {ex.Message}");
            }
        }

        public List<string> GetLog(int? tailCount = null)
        {
            return tailCount.HasValue ? _logEntries.TakeLast(tailCount.Value).ToList() : new List<string>(_logEntries);
        }

        private bool IsAppStateEqual(AppStateSnapShot state1, AppStateSnapShot state2)
        {
            if (state1 == null && state2 == null)
                return true;

            if (state1 == null || state2 == null)
                return false;

            if (state1.Guid != state2.Guid)
                return false;

            if (state1.TimeStamp != state2.TimeStamp)
                return false;

            if (state1.Jobs.Count != state2.Jobs.Count)
                return false;

            foreach (var job in state1.Jobs)
            {
                var correspondingJob = state2.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (correspondingJob == null || !job.Equals(correspondingJob))
                    return false;
            }

            if (state1.JobStateList.Count != state2.JobStateList.Count)
                return false;

            foreach (var kvp in state1.JobStateList)
            {
                if (!state2.JobStateList.ContainsKey(kvp.Key))
                    return false;

                var list1 = kvp.Value;
                var list2 = state2.JobStateList[kvp.Key];

                if (list1.Count != list2.Count)
                    return false;

                for (int i = 0; i < list1.Count; i++)
                {
                    if (!list1[i].Equals(list2[i]))
                        return false;
                }
            }

            return true;
        }
    }
}
