namespace SimpleMonitorTools.Models
{
    public enum PostLaunchStepType
    {
        InvokeControl,
        Sleep
    }

    public class PostLaunchStep
    {
        public PostLaunchStepType StepType { get; set; }

        // For InvokeControl
        public string WindowTitle { get; set; }
        public string ControlType { get; set; }
        public string Name { get; set; }
        public string NameMatchMode { get; set; }

        // For Sleep
        public int DurationMs { get; set; }
    }
}
