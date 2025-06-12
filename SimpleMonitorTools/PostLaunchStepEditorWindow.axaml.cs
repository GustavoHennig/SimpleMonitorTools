using Avalonia.Controls;
using SimpleMonitorTools.Models;

namespace SimpleMonitorTools
{
    public partial class PostLaunchStepEditorWindow : Window
    {
        public PostLaunchStepEditorWindow()
        {
            InitializeComponent();
        }

        public PostLaunchStepEditorWindow(System.Collections.Generic.IEnumerable<PostLaunchStep> steps = null) : this()
        {
            DataContext = new PostLaunchStepEditorViewModel(steps);
        }
    }
}
