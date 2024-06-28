using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WSyncProApp.Helpers;
using WSyncProApp.Models;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class ManageJobsView : UserControl
    {
        private const string JobsFilePath = "jobs.json";
        private List<Job> _jobs;

        public ManageJobsView()
        {
            InitializeComponent();
            LoadJobs();
        }

        private void LoadJobs()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JobsFilePath);
                _jobs = JsonHelper.LoadJobs(fullPath);

                if (_jobs != null && _jobs.Count > 0)
                {
                    JobsListView.ItemsSource = _jobs;
                    JobsListView.Visibility = Visibility.Visible;
                    NoJobsTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    JobsListView.Visibility = Visibility.Collapsed;
                    NoJobsTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (FileNotFoundException)
            {
                JobsListView.Visibility = Visibility.Collapsed;
                NoJobsTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError($"An error occurred while loading jobs: {ex.Message}");
                JobsListView.Visibility = Visibility.Collapsed;
                NoJobsTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void AddJob_Click(object sender, RoutedEventArgs e)
        {
            AddJobWindow addJobWindow = new AddJobWindow();
            if (addJobWindow.ShowDialog() == true)
            {
                _jobs.Add(addJobWindow.Job);
                JobsListView.Items.Refresh();

                // Save the new job list to the JSON file
                SaveJobs();
            }
        }

        private void EditJob_Click(object sender, RoutedEventArgs e)
        {
            Job selectedJob = (sender as Button).DataContext as Job;
            if (selectedJob != null)
            {
                EditJob(selectedJob);
            }
        }

        private void EditJob(Job job)
        {
            EditJobWindow editJobWindow = new EditJobWindow(job);
            if (editJobWindow.ShowDialog() == true)
            {
                // Update the list view after editing
                JobsListView.Items.Refresh();

                // Optionally, save the changes back to the JSON file
                SaveJobs();
            }
        }

        private void SaveJobs()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JobsFilePath);
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(_jobs, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(fullPath, jsonData);
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError($"An error occurred while saving jobs: {ex.Message}");
            }
        }
    }
}
