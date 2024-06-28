using System.Windows;
using WSyncProApp.Views;
namespace WSyncProApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new StartSyncView();
        }

        private void ManageJobs_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ManageJobsView();
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ViewHistoryView();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SettingsView();
        }
    }
}
