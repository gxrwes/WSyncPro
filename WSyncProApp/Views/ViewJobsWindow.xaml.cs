using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WSyncProApp.Models;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class ViewJobsWindow : Window
    {
        private List<Job> _jobs;

        public ViewJobsWindow(List<Job> jobs)
        {
            InitializeComponent();
            _jobs = jobs;
            JobsListView.ItemsSource = _jobs;
        }

        private void ViewJob_Click(object sender, RoutedEventArgs e)
        {
            Job selectedJob = (sender as Button).DataContext as Job;
            if (selectedJob != null)
            {
                ViewJobHistoryWindow viewJobHistoryWindow = new ViewJobHistoryWindow(selectedJob);
                viewJobHistoryWindow.ShowDialog();
            }
        }
    }
}
