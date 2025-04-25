using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleMonitorTools.Models
{
    public class Shortcut
    {

        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Executable path is required.")]
        public string ExecutablePath { get; set; }

        // Placeholder for monitor identifier. This will likely be updated based on Task 1 implementation.
        [Required(ErrorMessage = "Target monitor is required.")]
        public string TargetMonitor { get; set; }

        public bool RunOnStartup { get; set; } // Add RunOnStartup property

        public bool IsValid()
        {
            var context = new ValidationContext(this, null, null);
            var results = new System.Collections.Generic.List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }
    }
}
