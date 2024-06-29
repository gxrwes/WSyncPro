using System.ComponentModel;
using WSyncProApp.Models.WSyncProApp.Models;

namespace WSyncProApp.Models
{
    public class JobWrapper : INotifyPropertyChanged
    {
        private bool _isSelected;
        public Job Job { get; private set; }

        public JobWrapper(Job job)
        {
            Job = job;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public string Name => Job.Name;
        public int Priority => Job.Priority;
        public string Description => Job.Description;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
