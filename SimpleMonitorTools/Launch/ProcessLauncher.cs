using System;
using System.Diagnostics;
using SimpleMonitorTools.Models;

namespace SimpleMonitorTools.Launch
{
    public class ProcessLauncher
    {
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
                    // TODO: Implement window handling logic (Task 6)
                    Console.WriteLine($"Launched process: {shortcut.ExecutablePath} on monitor: {shortcut.TargetMonitor}");
                }
                else
                {
                    // TODO: Handle process start failure (Task 8)
                    Console.WriteLine($"Failed to launch process: {shortcut.ExecutablePath}");
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle exceptions (Task 8)
                Console.WriteLine($"Exception launching process: {ex.Message}");
            }
        }
    }
}
