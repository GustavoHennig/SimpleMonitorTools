using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading; // Required for Thread.Sleep
using SimpleMonitorTools.Models;

namespace SimpleMonitorTools.Launch
{
    public class ProcessLauncher
    {
        private readonly MonitorService _monitorService; // Add MonitorService field

        // P/Invoke declarations for window manipulation
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // SetWindowPos flags
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public ProcessLauncher(MonitorService monitorService) // Constructor to receive MonitorService
        {
            _monitorService = monitorService;
        }

        public void Launch(Shortcut shortcut)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = shortcut.ExecutablePath,
                    UseShellExecute = true // Required for launching with standard user privileges
                };

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    Console.WriteLine($"Launched process: {shortcut.ExecutablePath} on monitor: {shortcut.TargetMonitor}");

                    // Wait for the process's main window handle to become available
                    IntPtr hWnd = IntPtr.Zero;
                    int attempts = 0;
                    while (hWnd == IntPtr.Zero && attempts < 10) // Attempt to get handle for up to 10 seconds
                    {
                        process.Refresh(); // Refresh process information
                        hWnd = process.MainWindowHandle;
                        if (hWnd == IntPtr.Zero)
                        {
                            Thread.Sleep(1000); // Wait for 1 second before retrying
                            attempts++;
                        }
                    }

                    if (hWnd != IntPtr.Zero)
                    {
                        var monitorInfo = _monitorService.GetMonitorInfoByIdentifier(shortcut.TargetMonitor);

                        if (monitorInfo.HasValue)
                        {
                            var workArea = monitorInfo.Value.WorkArea;
                            // Position the window at the top-left of the monitor's working area
                            SetWindowPos(hWnd, IntPtr.Zero, workArea.left, workArea.top, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_SHOWWINDOW);
                            Console.WriteLine($"Positioned window for {shortcut.Name} on monitor {shortcut.TargetMonitor}");
                        }
                        else
                        {
                            Console.WriteLine($"Could not find monitor info for {shortcut.TargetMonitor}. Window not positioned.");
                            App.ShowNotification("Warning", $"Could not find monitor info for {shortcut.TargetMonitor}. Window not positioned.", Avalonia.Controls.Notifications.NotificationType.Warning);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not find main window handle for process: {shortcut.ExecutablePath} after multiple attempts. Window not positioned.");
                        App.ShowNotification("Warning", $"Could not find window for process: {shortcut.ExecutablePath}. Window not positioned.", Avalonia.Controls.Notifications.NotificationType.Warning);
                    }
                }
                else
                {
                    // TODO: Handle process start failure (Task 8)
                    Console.WriteLine($"Failed to launch process: {shortcut.ExecutablePath}");
                    App.ShowNotification("Error", $"Failed to launch process: {shortcut.ExecutablePath}", Avalonia.Controls.Notifications.NotificationType.Error); // Show error notification
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle exceptions (Task 8)
                Console.WriteLine($"Exception launching process: {ex.Message}");
                App.ShowNotification("Error", $"Exception launching process: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error); // Show error notification
            }
        }
    }
}
