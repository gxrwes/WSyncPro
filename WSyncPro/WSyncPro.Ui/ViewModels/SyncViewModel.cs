using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models;
using Microsoft.Maui.Controls;

namespace WSyncPro.Ui.ViewModels
{
    public partial class SyncViewModel : ObservableObject
    {
        // Collection of SyncJobs
        [ObservableProperty]
        private ObservableCollection<SyncJob> syncJobs;

        // Selected SyncJob
        [ObservableProperty]
        private SyncJob selectedSyncJob;

        // Track sorting state
        private string currentSortField = "";
        private bool isAscending = true;

        public SyncViewModel()
        {
            // Initialize with some sample data or leave empty
            SyncJobs = new ObservableCollection<SyncJob>();
        }

        // Command to Add a new Job
        [ICommand]
        private void AddJob()
        {
            // Create a new SyncJob with default values
            var newJob = new SyncJob
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New Sync Job",
                Description = "Description here...",
                IsEnabled = true,
                SrcDirectory = "",
                DstDirectory = "",
                FilesToSync = 0,
                TotalFilesSynced = 0,
                FailedFiles = 0
            };

            SyncJobs.Add(newJob);
        }

        // Command to Run Sync
        [ICommand]
        private async Task RunSyncAsync()
        {
            var enabledJobs = SyncJobs.Where(j => j.IsEnabled).ToList();

            if (enabledJobs.Count == 0)
            {
                // Display a message to the user
                await Shell.Current.DisplayAlert("Run Sync", "No enabled sync jobs to run.", "OK");
                return;
            }

            foreach (var job in enabledJobs)
            {
                // Call your StartSync method here
                // Example: await StartSync(job);
                // For demonstration, we'll simulate with a delay
                job.SyncStatus = "Syncing...";
                OnPropertyChanged(nameof(SyncJobs));

                await Task.Delay(500); // Simulate sync operation

                job.SyncStatus = "Sync Completed!";
                OnPropertyChanged(nameof(SyncJobs));
            }

            await Shell.Current.DisplayAlert("Run Sync", "Sync operations completed.", "OK");
        }

        // Command to Sort the SyncJobs
        [ICommand]
        private void Sort(string fieldName)
        {
            if (currentSortField == fieldName)
            {
                // Toggle sort direction
                isAscending = !isAscending;
            }
            else
            {
                currentSortField = fieldName;
                isAscending = true;
            }

            // Perform sorting
            var sortedList = isAscending
                ? SyncJobs.OrderBy(j => GetPropertyValue(j, fieldName)).ToList()
                : SyncJobs.OrderByDescending(j => GetPropertyValue(j, fieldName)).ToList();

            SyncJobs.Clear();
            foreach (var job in sortedList)
            {
                SyncJobs.Add(job);
            }
        }

        private object GetPropertyValue(SyncJob job, string propertyName)
        {
            return typeof(SyncJob).GetProperty(propertyName)?.GetValue(job, null);
        }
    }
}
