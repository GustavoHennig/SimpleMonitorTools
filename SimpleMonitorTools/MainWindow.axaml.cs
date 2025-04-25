using Avalonia.Controls;
using System.Collections.Generic;

namespace SimpleMonitorTools
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ManageShortcutsMenuItem.Click += ManageShortcutsMenuItem_Click;
            PopulateShortcuts();
        }

        private void ManageShortcutsMenuItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var shortcutManagerWindow = new ShortcutManagerWindow();
            shortcutManagerWindow.Show();
        }

        private void PopulateShortcuts()
        {
            // Placeholder data
            var shortcuts = new List<string>
            {
                "Shortcut 1: Open Calculator",
                "Shortcut 2: Open Notepad",
                "Shortcut 3: Open Paint"
            };

            ShortcutListBox.ItemsSource = shortcuts;
        }
    }
}
