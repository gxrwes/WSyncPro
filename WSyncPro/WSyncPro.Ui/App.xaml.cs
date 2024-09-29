// App.xaml.cs
using Microsoft.Maui.Controls;

namespace WSyncPro.Ui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the main shell
            MainPage = new AppShell();
        }
    }
}
