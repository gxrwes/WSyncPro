using System.Windows;
using Microsoft.Win32;
using WSyncPro.Core.Models;
using WSyncPro.Data.DataAccess;

namespace WSyncPro.UI.Views
{
    public partial class ImportProfileEditor : Window
    {
        private ImportProfile _profile;
        private readonly JsonDataAccess _dataAccess;

        public ImportProfileEditor()
        {
            InitializeComponent();
            _profile = new ImportProfile();
            _dataAccess = null; // This will be used for saving/loading a specific profile
        }

        public ImportProfileEditor(ImportProfile profile) : this()
        {
            _profile = profile;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            ProfileNameTextBox.Text = _profile.Name;
            DescriptionTextBox.Text = _profile.Description;
            DefaultImportPathTextBox.Text = _profile.DefaultImportPath;
            GroupingPatternTextBox.Text = _profile.GroupingPattern;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _profile.Name = ProfileNameTextBox.Text;
            _profile.Description = DescriptionTextBox.Text;
            _profile.DefaultImportPath = DefaultImportPathTextBox.Text;
            _profile.GroupingPattern = GroupingPatternTextBox.Text;

            // Logic to save the profile to a central list/database

            this.Close();
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Import Profile",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var dataAccess = new JsonDataAccess(saveFileDialog.FileName);
                dataAccess.SaveImportProfile(_profile);
                MessageBox.Show("Profile saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadProfileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Load Import Profile",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var dataAccess = new JsonDataAccess(openFileDialog.FileName);
                _profile = dataAccess.LoadImportProfile();
                if (_profile != null)
                {
                    LoadProfileData();
                    MessageBox.Show("Profile loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to load profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowseImportPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Import Path",
                Filter = "Folder Selection|*.none",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select folder and click Open"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DefaultImportPathTextBox.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
        }
    }
}
