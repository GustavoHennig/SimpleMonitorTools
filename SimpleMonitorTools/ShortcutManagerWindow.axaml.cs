using Avalonia.Controls;
using System.Threading.Tasks;

namespace SimpleMonitorTools
{

    public partial class ShortcutManagerWindow : Window
    {
        public ShortcutManagerWindow()
        {
            InitializeComponent();
            DataContext = new ShortcutManagerViewModel();
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
    }
}
