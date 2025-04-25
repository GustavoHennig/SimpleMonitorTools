using ReactiveUI;
using SimpleMonitorTools.Models;
using System.Collections.ObjectModel;
using System.Reactive;

namespace SimpleMonitorTools
{
    public class ShortcutManagerViewModel : ReactiveObject
    {
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

        public ReactiveCommand<Unit, Unit> AddShortcutCommand { get; }

        public ShortcutManagerViewModel()
        {
            Shortcuts = new ObservableCollection<Shortcut>();
            // Add some dummy data for now
            Shortcuts.Add(new Shortcut { Name = "Example Shortcut 1", ExecutablePath = "C:\\path\\to\\example1.exe" });
            Shortcuts.Add(new Shortcut { Name = "Example Shortcut 2", ExecutablePath = "C:\\path\\to\\example2.exe" });


            AddShortcutCommand = ReactiveCommand.Create(AddShortcut);
        }

        private void AddShortcut()
        {
            if (!string.IsNullOrWhiteSpace(NewShortcutName) && !string.IsNullOrWhiteSpace(NewShortcutPath))
            {
                Shortcuts.Add(new Shortcut { Name = NewShortcutName, ExecutablePath = NewShortcutPath });
                NewShortcutName = string.Empty;
                NewShortcutPath = string.Empty;
            }
        }
    }
}
