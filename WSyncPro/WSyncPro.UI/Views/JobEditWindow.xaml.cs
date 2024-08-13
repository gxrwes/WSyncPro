using System.Windows;
using Microsoft.Win32;
using WSyncPro.Core.Models;

namespace WSyncPro.UI.Views
{
    public partial class JobEditWindow : Window
    {
        public Job Job { get; private set; }

        public JobEditWindow(Job job = null)
        {
            InitializeComponent();

            if (job == null)
            {
                Job = new Job();
            }
            else
            {
                Job = job;
                LoadJobDetails();
            }
        }

        private void LoadJobDetails()
        {
            JobNameTextBox.Text = Job.Name;
            SourceDirectoryTextBox.Text = Job.SourceDirectory;
            TargetDirectoryTextBox.Text = Job.TargetDirectory;
            JobTypeComboBox.SelectedItem = Job.JobType.ToString();
            FileTypesTextBox.Text = string.Join(", ", Job.TargetedFileTypes);
            IsEnabledCheckBox.IsChecked = Job.IsEnabled;
            IsScheduledCheckBox.IsChecked = Job.IsScheduled;
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePath(SourceDirectoryTextBox.Text) || !ValidatePath(TargetDirectoryTextBox.Text))
            {
                MessageBox.Show("Please provide valid paths for source and target directories.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Job.Name = JobNameTextBox.Text;
            Job.SourceDirectory = SourceDirectoryTextBox.Text;
            Job.TargetDirectory = TargetDirectoryTextBox.Text;
            Job.JobType = (JobType)JobTypeComboBox.SelectedIndex;
            Job.TargetedFileTypes = FileTypesTextBox.Text.Split(new[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            Job.IsEnabled = IsEnabledCheckBox.IsChecked.GetValueOrDefault();
            Job.IsScheduled = IsScheduledCheckBox.IsChecked.GetValueOrDefault();

            DialogResult = true;
            Close();
        }

        private void CancelAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BrowseSourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = SelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                SourceDirectoryTextBox.Text = folderPath;
            }
        }

        private void BrowseTargetDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = SelectFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                TargetDirectoryTextBox.Text = folderPath;
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
                return System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
            return null;
        }

        private bool ValidatePath(string path)
        {
            return System.IO.Directory.Exists(path) || System.IO.File.Exists(path);
        }
    }
}
