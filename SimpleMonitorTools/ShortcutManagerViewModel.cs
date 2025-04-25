using ReactiveUI;
using SimpleMonitorTools.Models;
using SimpleMonitorTools.Persistence;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace SimpleMonitorTools
{
    public class ShortcutManagerViewModel : ReactiveObject
    {
        private readonly ShortcutRepository _repository;
        private readonly MonitorService _monitorService;

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

        private List<string> _availableMonitors;
        public List<string> AvailableMonitors
        {
            get => _availableMonitors;
            set => this.RaiseAndSetIfChanged(ref _availableMonitors, value);
        }

        private string _selectedMonitor;
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

        private Shortcut _selectedShortcut;
        public Shortcut SelectedShortcut
        {
            get => _selectedShortcut;
            set => this.RaiseAndSetIfChanged(ref _selectedShortcut, value);
        }

        public ReactiveCommand<Unit, Unit> AddShortcutCommand { get; }
        public ReactiveCommand<Models.Shortcut, Unit> RemoveShortcutCommand { get; }

        public ShortcutManagerViewModel(MonitorService monitorService)
        {
            _repository = new ShortcutRepository();
            this._monitorService = monitorService;

            var loadedShortcuts = _repository.LoadShortcuts();
            Shortcuts = new ObservableCollection<Shortcut>();
            loadedShortcuts.ForEach(f => Shortcuts.Add(f));

            AvailableMonitors = _monitorService.GetConnectedMonitors();
            SelectedMonitor = AvailableMonitors.FirstOrDefault() ?? "Default";

            AddShortcutCommand = ReactiveCommand.Create(AddShortcut);
            RemoveShortcutCommand = ReactiveCommand.Create<Models.Shortcut>(RemoveShortcut);

            // Subscribe to SelectedShortcut changes to populate NewShortcut fields
            this.WhenAnyValue(x => x.SelectedShortcut)
                .Subscribe(selectedShortcut =>
                {
                    if (selectedShortcut != null)
                    {
                        NewShortcutName = selectedShortcut.Name;
                        NewShortcutPath = selectedShortcut.ExecutablePath;
                        SelectedMonitor = selectedShortcut.TargetMonitor;
                        RunOnStartup = selectedShortcut.RunOnStartup;
                    }
                    else
                    {
                        NewShortcutName = string.Empty;
                        NewShortcutPath = string.Empty;
                        SelectedMonitor = AvailableMonitors.FirstOrDefault() ?? "Default";
                        RunOnStartup = false;
                    }
                });

            // Subscribe to changes in NewShortcut fields to auto-save when a shortcut is selected
            this.WhenAnyValue(
                x => x.NewShortcutName,
                x => x.NewShortcutPath,
                x => x.SelectedMonitor,
                x => x.RunOnStartup)
                .Throttle(TimeSpan.FromMilliseconds(500)) // Add a small delay to avoid saving on every keystroke
                .Where(_ => SelectedShortcut != null) // Only save if a shortcut is selected
                .ObserveOn(RxApp.MainThreadScheduler) // Ensure the subscription is on the UI thread
                .Subscribe(_ => SaveShortcut());
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
                    SelectedMonitor = AvailableMonitors.FirstOrDefault() ?? "Default";
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
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(newShortcut, null, null);
                var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
                System.ComponentModel.DataAnnotations.Validator.TryValidateObject(newShortcut, context, results, true);

                var errorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                App.ShowNotification("Validation Error", errorMessage, Avalonia.Controls.Notifications.NotificationType.Error);
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

        private void SaveShortcut()
        {
            if (SelectedShortcut != null)
            {
                var updatedShortcut = new Shortcut
                {
                    Name = NewShortcutName,
                    ExecutablePath = NewShortcutPath,
                    TargetMonitor = SelectedMonitor,
                    RunOnStartup = RunOnStartup
                };

                if (updatedShortcut.IsValid())
                {
                    try
                    {
                        var index = Shortcuts.IndexOf(SelectedShortcut);
                        if (index != -1)
                        {
                            // Handle RunOnStartup change
                            if (SelectedShortcut.RunOnStartup != updatedShortcut.RunOnStartup)
                            {
                                RegistryHelper.SetRunOnStartup(updatedShortcut.RunOnStartup, updatedShortcut.ExecutablePath);
                            }

                            // Update the existing shortcut object in the ObservableCollection
                            Shortcuts[index].Name = updatedShortcut.Name;
                            Shortcuts[index].ExecutablePath = updatedShortcut.ExecutablePath;
                            Shortcuts[index].TargetMonitor = updatedShortcut.TargetMonitor;
                            Shortcuts[index].RunOnStartup = updatedShortcut.RunOnStartup;


                            _repository.SaveShortcuts(Shortcuts.ToList());
                            App.ShowNotification("Success", "Shortcut updated successfully.", Avalonia.Controls.Notifications.NotificationType.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        App.ShowNotification("Error", $"Failed to update shortcut: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error);
                    }
                }
                else
                {
                    var context = new System.ComponentModel.DataAnnotations.ValidationContext(updatedShortcut, null, null);
                    var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
                    System.ComponentModel.DataAnnotations.Validator.TryValidateObject(updatedShortcut, context, results, true);

                    var errorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                    App.ShowNotification("Validation Error", errorMessage, Avalonia.Controls.Notifications.NotificationType.Error);
                }
            }
        }
    }
}
