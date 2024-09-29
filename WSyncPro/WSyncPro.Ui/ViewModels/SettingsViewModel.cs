// ViewModels/SettingsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WSyncPro.Models;
using Microsoft.Maui.Controls;

namespace WSyncPro.Ui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private SettingsModel settings;

        public SettingsViewModel()
        {
            // Initialize settings, possibly loading from persistent storage
            Settings = new SettingsModel
            {
                DefaultImportPath = "C:\\Imports",
                TrashDirecotry = "C:\\Trash",
                Version = "1.0.0",
                About = "WSyncPro - Synchronization Tool"
            };
        }

        [ICommand]
        private async Task SaveSettingsAsync()
        {
            // Implement saving logic here
            // For example, save to a file or database

            // For demonstration, show a confirmation
            await Shell.Current.DisplayAlert("Settings", "Settings saved successfully.", "OK");
        }

        [ICommand]
        private async Task LoadSettingsAsync()
        {
            // Implement loading logic here
            // For demonstration, do nothing
            await Task.CompletedTask;
        }
    }
}
