using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using WSyncProApp.Helpers;
using WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class SettingsView : UserControl
    {
        private AppSettings _appSettings;

        public SettingsView()
        {
            InitializeComponent();
            LoadSettings();
            LoadVersionInfo();
        }

        private void LoadSettings()
        {
            try
            {
                _appSettings = SettingsHelper.LoadSettings();
                DevModeCheckBox.IsChecked = _appSettings.DevMode;
                TrashDirectoryTextBox.Text = _appSettings.TrashDirectory;
                RunHistorySaveLocationTextBox.Text = _appSettings.RunHistorySaveLocation;
                SaveRunsForComboBox.SelectedValue = _appSettings.SaveRunsFor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVersionInfo()
        {
            // Placeholder for loading version info logic
            CurrentVersionText.Text = "1.0.0";
            LastReleaseNotesText.Text = "Initial release.";
            ReleaseNotesHistoryList.ItemsSource = new[]
            {
                new { Version = "1.0.0", Notes = "Initial release." }
            };
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _appSettings.DevMode = DevModeCheckBox.IsChecked ?? false;
                _appSettings.TrashDirectory = TrashDirectoryTextBox.Text;
                _appSettings.RunHistorySaveLocation = RunHistorySaveLocationTextBox.Text;
                _appSettings.SaveRunsFor = (SaveRunsForComboBox.SelectedValue as ComboBoxItem)?.Content.ToString();

                SettingsHelper.SaveSettings(_appSettings);
                MessageBox.Show("Settings saved successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseTrashDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folders|*.",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                TrashDirectoryTextBox.Text = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void BrowseRunHistorySaveLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folders|*.",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                RunHistorySaveLocationTextBox.Text = Path.GetDirectoryName(dialog.FileName);
            }
        }
    }
}
