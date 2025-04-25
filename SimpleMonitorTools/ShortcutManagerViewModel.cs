using ReactiveUI;
using SimpleMonitorTools.Models;
using SimpleMonitorTools.Persistence;
using System.Collections.Generic;
using System; // Add this using directive
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace SimpleMonitorTools
{
    public class ShortcutManagerViewModel : ReactiveObject
    {
        private readonly ShortcutRepository _repository;
        private readonly MonitorService _monitorService; // Add MonitorService field

        private ObservableCollection<Shortcut> _shortcuts;
        public ObservableCollection<Shortcut> Shortcuts
        {
            get => _shortcuts;
            set => this.RaiseAndSetIfChanged(ref _shortcuts, value);
        }

        private string _newShortcutName;
        public string NewShortcutName
        {
            get => _newShortcutName;
            set => this.RaiseAndSetIfChanged(ref _newShortcutName, value);
        }

        private string _newShortcutPath;
        public string NewShortcutPath
        {
            get => _newShortcutPath;
            set => this.RaiseAndSetIfChanged(ref _newShortcutPath, value);
        }

        private List<string> _availableMonitors; // Add AvailableMonitors property
        public List<string> AvailableMonitors
        {
            get => _availableMonitors;
            set => this.RaiseAndSetIfChanged(ref _availableMonitors, value);
        }

        private string _selectedMonitor; // Add SelectedMonitor property
        public string SelectedMonitor
        {
            get => _selectedMonitor;
            set => this.RaiseAndSetIfChanged(ref _selectedMonitor, value);
        }

        private bool _runOnStartup;
        public bool RunOnStartup
        {
            get => _runOnStartup;
            set => this.RaiseAndSetIfChanged(ref _runOnStartup, value);
        }

        public ReactiveCommand<Unit, Unit> AddShortcutCommand { get; }
        public ReactiveCommand<Models.Shortcut, Unit> RemoveShortcutCommand { get; } // Add RemoveShortcutCommand

        public ShortcutManagerViewModel()
        {
            _repository = new ShortcutRepository();
            _monitorService = new MonitorService();

            var loadedShortcuts = _repository.LoadShortcuts();
            Shortcuts = new ObservableCollection<Shortcut>();
            loadedShortcuts.ForEach(f=> Shortcuts.Add(f));
            

            AvailableMonitors = _monitorService.GetConnectedMonitors();
            SelectedMonitor = AvailableMonitors.FirstOrDefault() ?? "Default";

            AddShortcutCommand = ReactiveCommand.Create(AddShortcut);
            RemoveShortcutCommand = ReactiveCommand.Create<Models.Shortcut>(RemoveShortcut); // Initialize RemoveShortcutCommand
        }

        private void AddShortcut()
        {
            var newShortcut = new Shortcut 
            { 
                Name = NewShortcutName, 
                ExecutablePath = NewShortcutPath, 
                TargetMonitor = SelectedMonitor,
                RunOnStartup = RunOnStartup
            };

            if (newShortcut.IsValid())
            {
                try
                {
                    Shortcuts.Add(newShortcut);
                    _repository.SaveShortcuts(Shortcuts.ToList());
                    
                    if (RunOnStartup)
                    {
                        RegistryHelper.SetRunOnStartup(true, newShortcut.ExecutablePath);
                    }

                    NewShortcutName = string.Empty;
                    NewShortcutPath = string.Empty;
                    RunOnStartup = false;
                    
                    App.ShowNotification("Success", "Shortcut added successfully.", Avalonia.Controls.Notifications.NotificationType.Information);
                }
                catch (Exception ex)
                {
                    App.ShowNotification("Error", $"Failed to set run on startup: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error);
                }
            }
            else
            {
                // Add validation feedback to the user (Task 8)
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(newShortcut, null, null);
                var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
                System.ComponentModel.DataAnnotations.Validator.TryValidateObject(newShortcut, context, results, true);

                var errorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                App.ShowNotification("Validation Error", errorMessage, Avalonia.Controls.Notifications.NotificationType.Error); // Show validation error notification
            }
        }

        private void RemoveShortcut(Models.Shortcut shortcut)
        {
            if (shortcut != null)
            {
                try
                {
                    if (shortcut.RunOnStartup)
                    {
                        RegistryHelper.SetRunOnStartup(false);
                    }

                    Shortcuts.Remove(shortcut);
                    _repository.SaveShortcuts(Shortcuts.ToList());
                    App.ShowNotification("Success", "Shortcut removed successfully.", Avalonia.Controls.Notifications.NotificationType.Information);
                }
                catch (Exception ex)
                {
                    App.ShowNotification("Error", $"Failed to update startup settings: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error);
                }
            }
        }
    }
}
