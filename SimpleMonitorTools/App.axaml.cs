using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Markup.Xaml;
using System;

namespace SimpleMonitorTools
{
    public sealed class App : Application
    {
        private TrayIcon? _tray;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life)
            {
                _tray = new TrayIcon
                {
                    // TODO: Add application icon
                    // Icon = new WindowIcon(
                    //            new Bitmap(AssetLoader.Open(
                    //                new Uri("avares://SimpleMonitorTools/Assets/monitor.ico")))),
                    ToolTipText = "Simple Monitor Tool",
                    Menu = new NativeMenu
                    {
                        new NativeMenuItem("Launch"), // Placeholder for submenu
                        new NativeMenuItem("Manage Shortcuts...") { Command = new ShowManageShortcutsCommand() }, // Use Command for now
                        new NativeMenuItem("Reload Monitors") { Command = new ReloadMonitorsCommand() }, // Use Command for now
                        new NativeMenuItemSeparator(),
                        new NativeMenuItem("Exit") { Command = new ExitApplicationCommand(life) } // Use Command for now
                    }
                };

                TrayIcon.SetIcons(this, new TrayIcons { _tray }); // adds it
                life.Exit += (_, _) => _tray?.Dispose();          // tidy-up
            }

            base.OnFrameworkInitializationCompleted();
        }

        // Placeholder Commands (will be implemented later)
        private class ShowManageShortcutsCommand : System.Windows.Input.ICommand
        {
            public event EventHandler? CanExecuteChanged;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter)
            {
                var shortcutManagerWindow = new ShortcutManagerWindow();
                shortcutManagerWindow.Show();
            }
        }

        private class ReloadMonitorsCommand : System.Windows.Input.ICommand
        {
            public event EventHandler? CanExecuteChanged;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter)
            {
                // TODO: Implement monitor reload logic
            }
        }

        private class ExitApplicationCommand : System.Windows.Input.ICommand
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
    }
}
