using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using SimpleMonitorTools.Launch;
using SimpleMonitorTools.Persistence;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SimpleMonitorTools
{
    public sealed class App : Application
    {
        private TrayIcon? _tray;
        private ShortcutRepository _shortcutRepository;
        private ProcessLauncher _processLauncher;
        private WindowNotificationManager? _notificationManager;
        private MonitorService _monitorService; // Add MonitorService field

        public static WindowNotificationManager? NotificationManager { get; private set; }

        public App()
        {
            _shortcutRepository = new ShortcutRepository();
            _monitorService = new MonitorService(); // Initialize MonitorService
            _processLauncher = new ProcessLauncher(_monitorService); // Pass MonitorService to ProcessLauncher
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life)
            {
                // Initialize notification manager
                _notificationManager = new WindowNotificationManager(life.MainWindow);
                NotificationManager = _notificationManager; // Make it accessible

                BuildTrayMenu(life);

                TrayIcon.SetIcons(this, new TrayIcons { _tray }); // adds it
                life.Exit += (_, _) => _tray?.Dispose();          // tidy-up
            }

            base.OnFrameworkInitializationCompleted();
        }

        // Method to build the tray menu
        public void BuildTrayMenu(IClassicDesktopStyleApplicationLifetime life)
        {
            var launchSubmenu = new NativeMenu();
            var shortcuts = _shortcutRepository.LoadShortcuts();

            if (shortcuts.Count == 0)
            {
                launchSubmenu.Add(new NativeMenuItem("No Shortcuts Available"));
            }
            else
            {
                foreach (var shortcut in shortcuts)
                {
                    launchSubmenu.Add(new NativeMenuItem(shortcut.Name)
                    {
                        Command = new LaunchShortcutCommand(_processLauncher, shortcut)
                    });
                }
            }

            _tray = new TrayIcon
            {
                // TODO: Add application icon
                // Icon = new WindowIcon(
                //            new Bitmap(AssetLoader.Open(
                //                new Uri("avares://SimpleMonitorTools/Assets/monitor.ico")))),
                ToolTipText = "Simple Monitor Tool",
                Menu = new NativeMenu
                {
                    new NativeMenuItem("Launch") { Menu = launchSubmenu },
                    new NativeMenuItem("Manage Shortcuts...") { Command = new ShowManageShortcutsCommand(this) }, // Pass App instance
                    new NativeMenuItem("Reload Monitors") { Command = new ReloadMonitorsCommand() },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Exit") { Command = new ExitApplicationCommand(life) }
                }
            };
        }


        // Method to show a notification
        public static void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            NotificationManager?.Show(new Notification(title, message, type));
        }

        // Placeholder Commands (will be implemented later)
        private class ShowManageShortcutsCommand : ICommand
        {
            private readonly App _app;

            public event EventHandler? CanExecuteChanged;
            public bool CanExecute(object? parameter) => true;

            public ShowManageShortcutsCommand(App app)
            {
                _app = app;
            }

            public void Execute(object? parameter)
            {
                var shortcutManagerWindow = new ShortcutManagerWindow();
                shortcutManagerWindow.Closed += (sender, args) => // Handle window closed event
                {
                    if (_app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life)
                    {
                        _app.BuildTrayMenu(life); // Rebuild tray menu on close
                    }
                };
                shortcutManagerWindow.Show();
            }
        }

        private class ReloadMonitorsCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter)
            {
                // TODO: Implement monitor reload logic
            }
        }

        private class ExitApplicationCommand : ICommand
        {
            private readonly IClassicDesktopStyleApplicationLifetime _lifetime;
            public event EventHandler? CanExecuteChanged;
            public ExitApplicationCommand(IClassicDesktopStyleApplicationLifetime lifetime)
            {
                _lifetime = lifetime;
            }
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter)
            {
                _lifetime.Shutdown();
            }
        }

        private class LaunchShortcutCommand : ICommand
        {
            private readonly ProcessLauncher _processLauncher;
            private readonly Models.Shortcut _shortcut;

            public event EventHandler? CanExecuteChanged;

            public LaunchShortcutCommand(ProcessLauncher processLauncher, Models.Shortcut shortcut)
            {
                _processLauncher = processLauncher;
                _shortcut = shortcut;
            }

            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter)
            {
                _processLauncher.Launch(_shortcut);
            }
        }
    }
}
