using System.Collections.Generic;
using System.Windows;
using WSyncProApp.Models;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class ViewJobHistoryWindow : Window
    {
        public ViewJobHistoryWindow(Job job)
        {
            InitializeComponent();
            DataContext = job;
            // Placeholder: Replace with actual job log loading logic
            List<string> jobLog = new List<string>
            {
                "Log entry 1",
                "Log entry 2",
                "Log entry 3"
                // Add more log entries as needed
            };
            LogListView.ItemsSource = jobLog;
        }
    }
}
