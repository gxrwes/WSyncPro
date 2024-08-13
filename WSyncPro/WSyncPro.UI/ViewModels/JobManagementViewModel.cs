using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WSyncPro.Core.Models;
using WSyncPro.Data.DataAccess;
using WSyncPro.UI.Commands;
using WSyncPro.UI.Views;

namespace WSyncPro.UI.ViewModels
{
    public class JobManagementViewModel : BaseViewModel
    {
        private readonly JsonDataAccess _dataAccess;
        private ObservableCollection<Job> _jobs;

        public ObservableCollection<Job> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                OnPropertyChanged(nameof(Jobs));
            }
        }

        public ICommand CreateJobCommand { get; }
        public ICommand ImportJobsCommand { get; }
        public ICommand ExportJobsCommand { get; }
        public ICommand EditJobCommand { get; }
        public ICommand StartJobsCommand { get; }
        public ICommand ExecuteAllEnabledJobsCommand { get; }

        public JobManagementViewModel()
        {
            _dataAccess = new JsonDataAccess("jobs.json");
            Jobs = new ObservableCollection<Job>(_dataAccess.LoadJobs());

            CreateJobCommand = new RelayCommand(param => CreateJob());
            ImportJobsCommand = new RelayCommand(param => ImportJobs());
            ExportJobsCommand = new RelayCommand(param => ExportJobs());
            EditJobCommand = new RelayCommand<Job>(job => EditJob(job));
            StartJobsCommand = new RelayCommand(param => StartSelectedJobs());
            ExecuteAllEnabledJobsCommand = new RelayCommand(param => ExecuteAllEnabledJobs());
        }

        private void CreateJob()
        {
            var newJobWindow = new JobEditWindow();
            if (newJobWindow.ShowDialog() == true)
            {
                var newJob = newJobWindow.Job;
                Jobs.Add(newJob);
                _dataAccess.SaveJobs(Jobs.ToList());
            }
        }

        private void ImportJobs()
        {
            // Logic to import jobs from a file
        }

        private void ExportJobs()
        {
            // Logic to export jobs to a file
        }

        private void EditJob(Job job)
        {
            var editJobWindow = new JobEditWindow(job);
            if (editJobWindow.ShowDialog() == true)
            {
                _dataAccess.SaveJobs(Jobs.ToList());
            }
        }

        private void StartSelectedJobs()
        {
            // Logic to start selected jobs
        }

        private void ExecuteAllEnabledJobs()
        {
            foreach (var job in Jobs.Where(j => j.IsEnabled))
            {
                // Logic to execute job
            }
        }
    }
}
