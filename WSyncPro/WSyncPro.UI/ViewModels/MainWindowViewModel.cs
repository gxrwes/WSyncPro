using System.Collections.ObjectModel;
using WSyncPro.Core.Models;
using WSyncPro.Data.DataAccess;

namespace WSyncPro.UI.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly JsonDataAccess _dataAccess;

        public ObservableCollection<Job> Jobs { get; set; } = new ObservableCollection<Job>();
        public ObservableCollection<SyncRun> SyncRuns { get; set; } = new ObservableCollection<SyncRun>();

        public MainWindowViewModel()
        {
            _dataAccess = new JsonDataAccess("jobs.json");
            LoadData();
        }

        private void LoadData()
        {
            var jobs = _dataAccess.LoadJobs();
            foreach (var job in jobs)
            {
                Jobs.Add(job);
            }

            var syncRuns = _dataAccess.LoadSyncRuns();
            foreach (var run in syncRuns)
            {
                SyncRuns.Add(run);
            }
        }

        public void SaveData()
        {
            _dataAccess.SaveJobs(new List<Job>(Jobs));
            _dataAccess.SaveSyncRuns(new List<SyncRun>(SyncRuns));
        }
    }
}
