using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WSyncProApp.Managers;
using WSyncProApp.Models;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class StartSyncView : UserControl
    {
        private List<JobWrapper> _activeJobWrappers;

        public StartSyncView()
        {
            InitializeComponent();
            LoadActiveJobs();
            LoadPastSyncRuns();
        }

        private void LoadActiveJobs()
        {
            // Simulate loading jobs from a data source
            var activeJobs = GetActiveJobs();
            _activeJobWrappers = activeJobs.Select(job => new JobWrapper(job)).ToList();
            ActiveJobsListView.ItemsSource = _activeJobWrappers;
            ActiveJobsExpander.Header = $"Active Jobs (Total Jobs: {activeJobs.Count})";
        }

        private List<Job> GetActiveJobs()
        {
            // Placeholder: Replace with actual job loading logic
            return new List<Job>
            {
                new Job { Name = "Job 1", Priority = 1, Description = "Description 1", IsActive = true },
                new Job { Name = "Job 2", Priority = 2, Description = "Description 2", IsActive = true },
                // Add more jobs as needed
            };
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;
            foreach (var jobWrapper in _activeJobWrappers)
            {
                jobWrapper.IsSelected = isChecked;
            }
            ActiveJobsListView.Items.Refresh();
            UpdateTotalJobsSelected();
        }

        private void ActiveJobsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (JobWrapper job in e.RemovedItems)
            {
                job.IsSelected = false;
            }
            foreach (JobWrapper job in e.AddedItems)
            {
                job.IsSelected = true;
            }
            UpdateTotalJobsSelected();
        }

        private void UpdateTotalJobsSelected()
        {
            int selectedJobsCount = _activeJobWrappers.Count(jobWrapper => jobWrapper.IsSelected);
            TotalJobsSelectedTextBlock.Text = $"Total Number of Jobs Selected: {selectedJobsCount}";
        }

        private void VerifyJobsButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for VerifyJobs implementation
            MessageBox.Show("Verify Jobs clicked");
        }

        private void StartRunButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedJobs = _activeJobWrappers.Where(j => j.IsSelected).Select(j => j.Job).ToList();
            if (selectedJobs.Count == 0)
            {
                MessageBox.Show("No jobs selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var syncRun = new SyncRun
            {
                Jobs = selectedJobs,
                TimestampStart = DateTime.Now,
                RunMessages = new List<string> { "Sync run started." }
            };

            // Simulate running jobs and add messages
            foreach (var job in selectedJobs)
            {
                syncRun.RunMessages.Add($"Running job: {job.Name}");
                // Placeholder for job execution logic
            }

            syncRun.TimestampCompletion = DateTime.Now;
            syncRun.RunMessages.Add("Sync run completed.");

            SyncRunManager.SaveSyncRun(syncRun);

            MessageBox.Show("Sync run completed and saved.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadPastSyncRuns();
        }

        private void LoadPastSyncRuns()
        {
            var pastSyncRuns = SyncRunManager.LoadSyncRuns();
            PastSyncRunsListView.ItemsSource = pastSyncRuns;
        }
    }
}
