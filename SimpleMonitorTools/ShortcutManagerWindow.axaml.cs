using Avalonia.Controls;
using System.Threading.Tasks;
using Avalonia.Input;
using SimpleMonitorTools.Models;
using SimpleMonitorTools.Launch;

namespace SimpleMonitorTools
{

    public partial class ShortcutManagerWindow : Window
    {
        private readonly MonitorService _monitorService;

        public ShortcutManagerWindow(MonitorService monitorService)
        {
            InitializeComponent();
            DataContext = new ShortcutManagerViewModel(monitorService);
            linksDataGrid.DoubleTapped += LinksDataGrid_DoubleTapped; // Subscribe to DoubleTapped event
            this._monitorService = monitorService;
        }

        private async void BrowseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = await dialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                if (DataContext is ShortcutManagerViewModel viewModel)
                {
                    viewModel.NewShortcutPath = result[0];
                }
            }
        }

        private void LinksDataGrid_DoubleTapped(object sender, TappedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Shortcut selectedShortcut)
            {
                var processLauncher = new ProcessLauncher(_monitorService); // Create ProcessLauncher instance
                processLauncher.Launch(selectedShortcut); // Launch the shortcut
            }
        }
    }
}
