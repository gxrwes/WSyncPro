﻿using System.IO;
using System.Windows;
using Microsoft.Win32;
using WSyncPro.Core.State;

namespace WSyncPro.UI.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly StateManager _stateManager;

        public SettingsWindow()
        {
            InitializeComponent();
            _stateManager = StateManager.Instance;
            LoadSettings();
        }

        private void LoadSettings()
        {
            var appSettings = _stateManager.AppSettings;
            if (appSettings != null)
            {
                HandBrakeCliPathTextBox.Text = appSettings.HandBrakeCliPath;
                CleanJobTargetDirectoryTextBox.Text = appSettings.TrashDirectory;
                ArchiveDirectoryTextBox.Text = appSettings.DefaultArchiveDirectory;
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

            var appSettings = _stateManager.AppSettings;

            appSettings.HandBrakeCliPath = HandBrakeCliPathTextBox.Text;
            appSettings.TrashDirectory = CleanJobTargetDirectoryTextBox.Text;
            appSettings.DefaultArchiveDirectory = ArchiveDirectoryTextBox.Text;

            _stateManager.SaveSettings();
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
