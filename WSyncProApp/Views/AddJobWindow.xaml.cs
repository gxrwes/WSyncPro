using System;
using System.Windows;
using WSyncProApp.Models;
using WSyncProApp.Helpers;
using System.Windows.Controls;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class AddJobWindow : Window
    {
        public Job Job { get; private set; }

        public AddJobWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriorityTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(SourceDirectoryTextBox.Text) ||
                string.IsNullOrWhiteSpace(TargetDirectoryTextBox.Text) ||
                ModeComboBox.SelectedItem == null)
            {
                ErrorHandler.ShowError("All fields must be filled out.");
                return;
            }

            try
            {
                Job = new Job
                {
                    Name = NameTextBox.Text,
                    Id = Guid.NewGuid(),
                    Priority = int.Parse(PriorityTextBox.Text),
                    Description = DescriptionTextBox.Text,
                    SourceDirectory = SourceDirectoryTextBox.Text,
                    TargetDirectory = TargetDirectoryTextBox.Text,
                    Mode = (ModeComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                    IsActive = IsActiveCheckBox.IsChecked ?? false,
                    LastRunDate = DateTime.MinValue,
                    TargetedFiletypes = new List<string>(),
                    Filter = string.Empty,
                    History = new List<string>()
                };

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError($"An error occurred while adding the job: {ex.Message}");
            }
        }
    }
}
