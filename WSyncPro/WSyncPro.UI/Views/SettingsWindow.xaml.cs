using System.IO;
using System.Windows;
using Microsoft.Win32;
using WSyncPro.Core.Models;
using WSyncPro.Data.DataAccess;

namespace WSyncPro.UI.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly JsonDataAccess _dataAccess;

        public SettingsWindow()
        {
            InitializeComponent();
            _dataAccess = new JsonDataAccess("settings.json");
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _dataAccess.LoadSettings();
            if (settings != null)
            {
                HandBrakeCliPathTextBox.Text = settings.HandBrakeCliPath;
                CleanJobTargetDirectoryTextBox.Text = settings.CleanJobTargetDirectory;
                ArchiveDirectoryTextBox.Text = settings.ArchiveDirectory;
                GroupingPatternTextBox.Text = settings.GroupingPattern;
            }
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePath(HandBrakeCliPathTextBox.Text) ||
                !ValidatePath(CleanJobTargetDirectoryTextBox.Text) ||
                !ValidatePath(ArchiveDirectoryTextBox.Text))
            {
                MessageBox.Show("One or more paths are invalid. Please correct them and try again.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var settings = new ApplicationSettings
            {
                HandBrakeCliPath = HandBrakeCliPathTextBox.Text,
                CleanJobTargetDirectory = CleanJobTargetDirectoryTextBox.Text,
                ArchiveDirectory = ArchiveDirectoryTextBox.Text,
                GroupingPattern = GroupingPatternTextBox.Text
            };

            _dataAccess.SaveSettings(settings);
            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Close the settings window
        }

        private void CancelAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the settings window without saving
        }

        private bool ValidatePath(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        private void BrowseHandBrakeCliPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select HandBrake CLI Executable",
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                HandBrakeCliPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseCleanJobTargetDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = SelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                CleanJobTargetDirectoryTextBox.Text = folderPath;
            }
        }

        private void BrowseArchiveDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = SelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                ArchiveDirectoryTextBox.Text = folderPath;
            }
        }

        private string SelectFolder()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Folder",
                Filter = "Folder Selection|*.none",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select folder and click Open"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return Path.GetDirectoryName(openFileDialog.FileName);
            }
            return null;
        }
    }
}
