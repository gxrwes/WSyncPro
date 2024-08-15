using System.IO;
using System.Windows;
using Microsoft.Win32;
using WSyncPro.Core.State;

namespace WSyncPro.UI.Views
{
    public partial class SetupWindow : Window
    {
        private readonly StateManager _stateManager;

        public SetupWindow()
        {
            InitializeComponent();
            _stateManager = StateManager.Instance;
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load existing settings, if any
            var appSettings = _stateManager.AppSettings;

            HandBrakeCliPathTextBox.Text = appSettings.HandBrakeCliPath;
            TrashDirectoryTextBox.Text = appSettings.TrashDirectory;
            ArchiveDirectoryTextBox.Text = appSettings.DefaultArchiveDirectory;
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePath(HandBrakeCliPathTextBox.Text) ||
                !ValidatePath(TrashDirectoryTextBox.Text) ||
                !ValidatePath(ArchiveDirectoryTextBox.Text))
            {
                MessageBox.Show("One or more paths are invalid. Please correct them and try again.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save the settings to StateManager
            var appSettings = _stateManager.AppSettings;

            appSettings.HandBrakeCliPath = HandBrakeCliPathTextBox.Text;
            appSettings.TrashDirectory = TrashDirectoryTextBox.Text;
            appSettings.DefaultArchiveDirectory = ArchiveDirectoryTextBox.Text;

            _stateManager.SaveSettings();
            MessageBox.Show("Setup completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Optionally, handle what happens if the user cancels the setup.
            this.Close();
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

        private void BrowseTrashDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = SelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                TrashDirectoryTextBox.Text = folderPath;
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
