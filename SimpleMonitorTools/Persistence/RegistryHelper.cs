using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace SimpleMonitorTools.Persistence
{
    public static class RegistryHelper
    {
        private const string RegistryRunPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "SimpleMonitorTool";

        public static void SetRunOnStartup(bool enable, string executablePath = null)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, true))
                {
                    if (enable && !string.IsNullOrEmpty(executablePath))
                    {
                        key.SetValue(AppName, $"\"{executablePath}\"");
                    }
                    else
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting run on startup: {ex.Message}");
                throw;
            }
        }

        public static bool IsRunOnStartupEnabled()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, false))
                {
                    return key.GetValue(AppName) != null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking run on startup: {ex.Message}");
                return false;
            }
        }
    }
}
