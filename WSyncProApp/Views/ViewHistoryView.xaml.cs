using System.Windows;
using System.Windows.Controls;
using WSyncProApp.Managers;
using WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class ViewHistoryView : UserControl
    {
        public ViewHistoryView()
        {
            InitializeComponent();
            LoadSyncRunHistory();
        }

        private void LoadSyncRunHistory()
        {
            var syncRuns = SyncRunManager.LoadSyncRuns();
            SyncRunsListView.ItemsSource = syncRuns;
        }

        private void ViewJobsButton_Click(object sender, RoutedEventArgs e)
        {
            SyncRun selectedSyncRun = (sender as Button).DataContext as SyncRun;
            if (selectedSyncRun != null)
            {
                ViewJobsWindow viewJobsWindow = new ViewJobsWindow(selectedSyncRun.Jobs);
                viewJobsWindow.ShowDialog();
            }
        }
    }
}
