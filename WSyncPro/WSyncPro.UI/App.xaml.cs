using System.Windows;
using WSyncPro.Core.State;

namespace WSyncPro
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize StateManager
            var stateManager = StateManager.Instance;

            // Load the state (if not already done in the constructor)
            stateManager.LoadState();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save the state before exiting
            StateManager.Instance.SaveState();

            base.OnExit(e);
        }
    }
}
