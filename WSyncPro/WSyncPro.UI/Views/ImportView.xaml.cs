using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WSyncPro.Core.Models;
using WSyncPro.Core.Services;
using WSyncPro.Core.Services.ScanService;
using WSyncPro.Data.DataAccess;

namespace WSyncPro.UI.Views
{
    public partial class ImportView : UserControl
    {
        private readonly ExternalDeviceDetectorService _deviceDetector;
        private readonly List<ImportProfile> _importProfiles;

        public ImportView()
        {
            _deviceDetector = new ExternalDeviceDetectorService();
            _importProfiles = LoadImportProfiles();

            InitializeComponent();
            LoadDevices();
            LoadProfiles();
        }

        private void LoadDevices()
        {
            var devices = _deviceDetector.GetExternalUsbDevices();
            if (devices != null && devices.Count > 0)
            {
                DeviceComboBox.ItemsSource = devices;
                DeviceComboBox.SelectedIndex = 0; // Optionally select the first device
            }
            else
            {
                MessageBox.Show("No external devices detected. Please connect a USB device.");
            }
        }

        private void LoadProfiles()
        {
            ProfileComboBox.ItemsSource = _importProfiles;
            ProfileComboBox.DisplayMemberPath = "Name";
            ProfileComboBox.SelectedIndex = 0; // Optionally select the first profile
        }

        private List<ImportProfile> LoadImportProfiles()
        {
            var dataAccess = new JsonDataAccess("ImportProfiles.json");
            return dataAccess.LoadImportProfiles() ?? new List<ImportProfile>();
        }

        private void RefreshDevices_Click(object sender, RoutedEventArgs e)
        {
            LoadDevices(); // Refresh the list of external devices
        }

        private void NewProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileEditor = new ImportProfileEditor();
            profileEditor.ShowDialog();

            // Reload profiles after creating a new one
            _importProfiles.Clear();
            _importProfiles.AddRange(LoadImportProfiles());
            LoadProfiles();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var selectedProfile = ProfileComboBox.SelectedItem as ImportProfile;
            if (selectedProfile != null)
            {
                var profileEditor = new ImportProfileEditor(selectedProfile);
                profileEditor.ShowDialog();

                // Reload profiles after editing
                _importProfiles.Clear();
                _importProfiles.AddRange(LoadImportProfiles());
                LoadProfiles();
            }
            else
            {
                MessageBox.Show("Please select a profile to edit.");
            }
        }

        private void ImportProfileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Import Profile",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var dataAccess = new JsonDataAccess(openFileDialog.FileName);
                var profile = dataAccess.LoadImportProfile();
                if (profile != null)
                {
                    _importProfiles.Add(profile);
                    LoadProfiles();
                    MessageBox.Show("Profile imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to import profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDevice = DeviceComboBox.SelectedItem as UsbDeviceInfo;
            var selectedProfile = ProfileComboBox.SelectedItem as ImportProfile;

            if (selectedDevice == null || selectedProfile == null)
            {
                MessageBox.Show("Please select both a device and an import profile.");
                return;
            }

            var job = new Job
            {
                SourceDirectory = selectedDevice.Name,
                TargetDirectory = selectedProfile.DefaultImportPath,
                TargetedFileTypes = selectedProfile.TargetedFileTypes,
                FilterStrings = selectedProfile.FilterStrings,
                ArchiveOptions = new ArchiveOptions { GroupingPattern = selectedProfile.GroupingPattern }
            };

            var importService = new ImportService(new CopyService(), new DirectoryScanner());
            importService.ExecuteImport(job, selectedDevice);
            MessageBox.Show("Import completed.");
        }
    }
}
