using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace SimpleMonitorTools.Persistence
{
    public static class RegistryHelper
    {
        private const string RegistryRunPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "SimpleMonitorTool";

        public static void SetRunOnStartup(bool enable, string executablePath = null, string registryKeyName = null)
        {
            string keyName = registryKeyName ?? AppName;
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, true))
                {
                    if (enable && !string.IsNullOrEmpty(executablePath))
                    {
                        key.SetValue(keyName, $"\"{executablePath}\"");
                    }
                    else
                    {
                        key.DeleteValue(keyName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting run on startup: {ex.Message}");
                throw;
            }
        }

        public static bool IsRunOnStartupEnabled(string registryKeyName = null)
        {
            string keyName = registryKeyName ?? AppName;
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, false))
                {
                    return key.GetValue(keyName) != null;
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
