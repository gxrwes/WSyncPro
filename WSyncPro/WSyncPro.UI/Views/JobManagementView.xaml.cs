using System.Windows.Controls;

namespace WSyncPro.UI.Views
{
    public partial class JobManagementView : UserControl
    {
        public JobManagementView()
        {
            InitializeComponent();
            DataContext = new WSyncPro.UI.ViewModels.JobManagementViewModel();
        }
    }
}
