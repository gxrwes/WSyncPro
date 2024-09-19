namespace WSyncPro.Ui.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ImportViewModel ImportVM { get; } = new ImportViewModel();
        public SyncViewModel SyncVM { get; } = new SyncViewModel();
        public SettingsViewModel SettingsVM { get; } = new SettingsViewModel();

        // Optionally, you can keep this greeting or remove it
        public string Greeting => "Welcome to Avalonia!";
    }
}
