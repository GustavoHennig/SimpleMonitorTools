using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Collections.Generic; // Add this using directive

namespace SimpleMonitorTools.Models
{
    public class Shortcut : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _executablePath;
        [Required(ErrorMessage = "Executable path is required.")]
        public string ExecutablePath
        {
            get => _executablePath;
            set => SetProperty(ref _executablePath, value);
        }

        private string _targetMonitor;
        // Placeholder for monitor identifier. This will likely be updated based on Task 1 implementation.
        [Required(ErrorMessage = "Target monitor is required.")]
        public string TargetMonitor
        {
            get => _targetMonitor;
            set => SetProperty(ref _targetMonitor, value);
        }

        private bool _runOnStartup;
        public bool RunOnStartup
        {
            get => _runOnStartup;
            set => SetProperty(ref _runOnStartup, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool IsValid()
        {
            var context = new ValidationContext(this, null, null);
            var results = new System.Collections.Generic.List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }
    }
}
