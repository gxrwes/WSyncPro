using System;
using System.Windows;
using WSyncProApp.Models;
using WSyncProApp.Helpers;
using System.Windows.Controls;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Views
{
    public partial class EditJobWindow : Window
    {
        public Job Job { get; set; }

        public EditJobWindow(Job job)
        {
            InitializeComponent();
            Job = job;
            LoadJobDetails();
        }

        private void LoadJobDetails()
        {
            NameTextBox.Text = Job.Name;
            PriorityTextBox.Text = Job.Priority.ToString();
            DescriptionTextBox.Text = Job.Description;
            SourceDirectoryTextBox.Text = Job.SourceDirectory;
            TargetDirectoryTextBox.Text = Job.TargetDirectory;
            ModeComboBox.SelectedItem = Job.Mode;
            IsActiveCheckBox.IsChecked = Job.IsActive;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Job.Name = NameTextBox.Text;
                Job.Priority = int.Parse(PriorityTextBox.Text);
                Job.Description = DescriptionTextBox.Text;
                Job.SourceDirectory = SourceDirectoryTextBox.Text;
                Job.TargetDirectory = TargetDirectoryTextBox.Text;
                Job.Mode = (ModeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                Job.IsActive = IsActiveCheckBox.IsChecked ?? false;

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError($"An error occurred while saving the job! Please Verify that all fields are filled\nDetailed Message: {ex.Message}");
            }
        }
    }
}
