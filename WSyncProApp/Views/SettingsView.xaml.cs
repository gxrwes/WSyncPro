using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WSyncProApp.Helpers;
using WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class SettingsView : UserControl
    {
        private const string VersionInfoPath = "version.json";

        public SettingsView()
        {
            InitializeComponent();
            LoadVersionInfo();
        }

        private void LoadVersionInfo()
        {
            try
            {
                // Get the path relative to the executable location
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VersionInfoPath);
                var versionInfo = JsonHelper.LoadVersionInfo(fullPath);

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
