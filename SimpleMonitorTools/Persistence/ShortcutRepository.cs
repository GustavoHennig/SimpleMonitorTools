using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SimpleMonitorTools.Models;

namespace SimpleMonitorTools.Persistence
{
    public class ShortcutRepository
    {
        private readonly string _appDataPath;
        private readonly string _shortcutsFilePath;

        public ShortcutRepository()
        {
            _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleMonitorTool");
            _shortcutsFilePath = Path.Combine(_appDataPath, "shortcuts.json");
        }

        public List<Shortcut> LoadShortcuts()
        {
            if (!File.Exists(_shortcutsFilePath))
            {
                return new List<Shortcut>();
            }

            try
            {
                string jsonString = File.ReadAllText(_shortcutsFilePath);
                var jsonContent = JsonSerializer.Deserialize<ShortcutsFileContent>(jsonString);
                return jsonContent?.shortcuts ?? new List<Shortcut>();
            }
            catch (Exception ex)
            {
                // TODO: Log the error (Task 8)
                Console.WriteLine($"Error loading shortcuts: {ex.Message}");
                return new List<Shortcut>();
            }
        }

        public void SaveShortcuts(List<Shortcut> shortcuts)
        {
            try
            {
                Directory.CreateDirectory(_appDataPath);

                var jsonContent = new ShortcutsFileContent
                {
                    version = 1, // According to specification
                    shortcuts = shortcuts
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(jsonContent, options);
                File.WriteAllText(_shortcutsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                // TODO: Log the error (Task 8)
                Console.WriteLine($"Error saving shortcuts: {ex.Message}");
                App.ShowNotification("Error", $"Failed to save shortcuts: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error); // Show error notification
            }
        }

        private class ShortcutsFileContent
        {
            public int version { get; set; }
            public List<Shortcut> shortcuts { get; set; }
        }
    }
}
