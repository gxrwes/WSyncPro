// AppStateManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WSyncPro.Core.Services;
using WSyncPro.Models;
using WSyncPro.Models.Enums;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Managers
{
    /// <summary>
    /// Manages the application's state, including loading, saving, and providing access to the current state.
    /// Implements the Singleton pattern to ensure a single instance throughout the application.
    /// </summary>
    public sealed class AppStateManager
    {
        private static readonly Lazy<AppStateManager> lazy =
            new Lazy<AppStateManager>(() => new AppStateManager());

        /// <summary>
        /// Gets the singleton instance of the <see cref="AppStateManager"/>.
        /// </summary>
        public static AppStateManager Instance { get { return lazy.Value; } }

        /// <summary>
        /// Gets the list of loaded jobs.
        /// </summary>
        public List<Job> Jobs { get; private set; }

        /// <summary>
        /// Holds the current snapshot of the application's state.
        /// </summary>
        private AppStateSnapShot _currentAppState;

        /// <summary>
        /// Path to the file where the app state is saved.
        /// </summary>
        private string _appStateFilePath;

        /// <summary>
        /// Service responsible for serializing and deserializing objects to and from JSON files.
        /// </summary>
        private readonly FileSerialisationServiceJson _serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStateManager"/> class.
        /// Loads the existing app state or initializes a new one if none exists.
        /// </summary>
        private AppStateManager()
        {
            // Initialize the jobs list
            Jobs = new List<Job>();

            // Define the path to the app state file
            string appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WSyncPro");

            // Ensure the directory exists
            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }

            _appStateFilePath = Path.Combine(appDataDirectory, "AppState.json");

            // Initialize the serialization service
            _serializationService = new FileSerialisationServiceJson();

            // Load the existing app state if available
            LoadAppState();
        }

        /// <summary>
        /// Updates the current application state based on the loaded jobs and their progress.
        /// </summary>
        public void UpdateAppState()
        {
            var newAppState = new AppStateSnapShot
            {
                Guid = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Jobs = Jobs.ToList() // Copy jobs
            };

            foreach (var job in Jobs)
            {
                if (!newAppState.JobStateList.ContainsKey(job.Id))
                {
                    newAppState.JobStateList[job.Id] = new List<JobProgress>();
                }

                newAppState.JobStateList[job.Id] = job.ProgressHistory.ToList();
            }

            // Check if the new state differs from the current state
            if (!IsAppStateEqual(_currentAppState, newAppState))
            {
                _currentAppState = newAppState;
                SaveAppState();
            }
        }

        /// <summary>
        /// Retrieves the current application state snapshot.
        /// </summary>
        /// <returns>The current <see cref="AppStateSnapShot"/>.</returns>
        public AppStateSnapShot GetAppState()
        {
            return _currentAppState;
        }

        /// <summary>
        /// Saves the current application state to a JSON file.
        /// The state is only saved if it has changed since the last save.
        /// </summary>
        private void SaveAppState()
        {
            try
            {
                // Serialize the current app state
                bool saveResult = _serializationService.SaveClassToFileAsync(_appStateFilePath, _currentAppState).Result;

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
                // Handle exceptions (e.g., logging)
                Console.WriteLine($"Error saving app state: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the most recent application state from the JSON file.
        /// If no state exists, initializes a new snapshot.
        /// </summary>
        public void LoadAppState()
        {
            try
            {
                if (File.Exists(_appStateFilePath))
                {
                    _currentAppState = _serializationService.GetFileAsClassAsync<AppStateSnapShot>(_appStateFilePath).Result;
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

                // Reconstruct Jobs list from JobStateList
                if (_currentAppState.Jobs != null)
                {
                    Jobs = _currentAppState.Jobs.ToList();
                }
                else
                {
                    Jobs = new List<Job>();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., logging)
                Console.WriteLine($"Error loading app state: {ex.Message}");
                _currentAppState = new AppStateSnapShot
                {
                    Guid = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Determines whether two <see cref="AppStateSnapShot"/> instances are equal.
        /// </summary>
        /// <param name="state1">The first app state snapshot.</param>
        /// <param name="state2">The second app state snapshot.</param>
        /// <returns><c>true</c> if the states are equal; otherwise, <c>false</c>.</returns>
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

            // Compare each job
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
