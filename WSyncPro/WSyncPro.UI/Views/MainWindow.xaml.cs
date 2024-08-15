using System.Windows;
using WSyncPro.Core.Services;

namespace WSyncPro.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
