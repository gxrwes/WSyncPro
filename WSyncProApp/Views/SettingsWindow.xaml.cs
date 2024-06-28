using System.Windows;
using System.Windows.Controls;

namespace WSyncProApp.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string selectedTheme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedTheme == "Light Theme")
            {
                ApplyTheme("pack://application:,,,/MaterialDesignThemes;component/Themes/MaterialDesignTheme.Light.xaml");
            }
            else if (selectedTheme == "Dark Theme")
            {
                ApplyTheme("pack://application:,,,/MaterialDesignThemes;component/Themes/MaterialDesignTheme.Dark.xaml");
            }
        }

        private void ApplyTheme(string themeUri)
        {
            var resourceDictionary = new ResourceDictionary() { Source = new Uri(themeUri) };

            // Clear existing ResourceDictionary from Application
            Application.Current.Resources.MergedDictionaries.Clear();

            // Add new ResourceDictionary to Application
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
