using System.IO;
using System.Windows;
using WSyncProApp.Helpers;
using WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class SettingsWindow : Window
    {
        private const string VersionInfoPath = "version.json";

        public SettingsWindow()
        {
            InitializeComponent();
            LoadVersionInfo();
        }

        private void LoadVersionInfo()
        {
            try
            {
                var versionInfo = JsonHelper.LoadVersionInfo(VersionInfoPath);

                CurrentVersionText.Text = versionInfo.CurrentVersion;
                LastReleaseNotesText.Text = versionInfo.LastReleaseNotes;
                ReleaseNotesHistoryList.ItemsSource = versionInfo.ReleaseNotesHistory;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
