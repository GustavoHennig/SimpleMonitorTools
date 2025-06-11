using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading; // Required for Thread.Sleep
using Avalonia.Threading;
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

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // SetWindowPos flags
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOACTIVATE = 0x0010;

        public ProcessLauncher(MonitorService monitorService) // Constructor to receive MonitorService
        {
            _monitorService = monitorService;
        }

        static IntPtr FindWindowByProcessId(int processId)
        {
            IntPtr foundHwnd = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint windowProcessId);
                if (windowProcessId == processId)
                {
                    foundHwnd = hWnd;
                    return false; // Stop enumeration
                }
                return true; // Continue
            }, IntPtr.Zero);

            return foundHwnd;
        }

        static IntPtr FindApplicationFrameHostWindow(int uwpProcessId)
        {
            IntPtr foundHostHwnd = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                uint pid;
                GetWindowThreadProcessId(hWnd, out pid);

                // Only care about ApplicationFrameHost.exe windows
                if (IsApplicationFrameHost(pid))
                {
                    // Search inside child windows
                    EnumChildWindows(hWnd, (childHwnd, lParam2) =>
                    {
                        uint childPid;
                        GetWindowThreadProcessId(childHwnd, out childPid);

                        if (childPid == uwpProcessId)
                        {
                            foundHostHwnd = hWnd;
                            return false; // Stop child search
                        }

                        return true; // Continue child search
                    }, IntPtr.Zero);

                    if (foundHostHwnd != IntPtr.Zero)
                        return false; // Stop main window search
                }

                return true; // Continue enumerating top-level windows
            }, IntPtr.Zero);

            return foundHostHwnd;
        }

        static bool IsApplicationFrameHost(uint processId)
        {
            try
            {
                var proc = Process.GetProcessById((int)processId);
                return proc.ProcessName.Equals("ApplicationFrameHost", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
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
                    while (hWnd == IntPtr.Zero && attempts < 50) // Attempt to get handle for up to 10 seconds
                    {
                        process.Refresh(); // Refresh process information
                        hWnd = process.MainWindowHandle;
                        if (hWnd == IntPtr.Zero)
                        {
                            hWnd = FindApplicationFrameHostWindow(process.Id);
                            if (hWnd == IntPtr.Zero)
                            {

                                Thread.Sleep(100); // Wait for 1 second before retrying
                                Dispatcher.UIThread.RunJobs();
                                attempts++;
                            }

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

                            // Execute post-launch steps
                            if (shortcut.PostLaunchSteps != null)
                            {
                                foreach (var step in shortcut.PostLaunchSteps)
                                {
                                    if (step.StepType == PostLaunchStepType.InvokeControl)
                                    {
                                        // Parse ControlType and NameMatchMode from string to enum
                                        var controlType = (FlaUI.Core.Definitions.ControlType)Enum.Parse(typeof(FlaUI.Core.Definitions.ControlType), step.ControlType);
                                        var matchMode = (SimpleMonitorTools.NameMatchMode)Enum.Parse(typeof(SimpleMonitorTools.NameMatchMode), step.NameMatchMode);

                                        new ExternalWinUiInteractor().InvokeControl(
                                            step.WindowTitle,
                                            controlType,
                                            step.Name,
                                            matchMode
                                        );
                                    }
                                    else if (step.StepType == PostLaunchStepType.Sleep)
                                    {
                                        Thread.Sleep(step.DurationMs);
                                    }
                                }
                            }
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
