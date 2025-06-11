using Avalonia.Controls;
using SimpleMonitorTools.Models;

namespace SimpleMonitorTools
{
    public partial class PostLaunchStepEditorWindow : Window
    {
        public PostLaunchStepEditorWindow(System.Collections.Generic.IEnumerable<PostLaunchStep> steps = null)
        {
            InitializeComponent();
            DataContext = new PostLaunchStepEditorViewModel(steps);
        }
    }
}
