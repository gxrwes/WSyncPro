using Microsoft.Maui.Controls;

namespace WSyncPro.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the MainPage to your Blazor WebView Page
            MainPage = new MainPage();
        }
    }
}
