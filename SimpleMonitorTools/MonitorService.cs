using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleMonitorTools
{
    public class MonitorService
    {
        // P/Invoke declarations for monitor enumeration
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // P/Invoke declaration for getting monitor info
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        // P/Invoke declaration for enumerating display devices
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }


        // Struct to hold detailed monitor information
        public struct MonitorInfo
        {
            public IntPtr HMonitor { get; set; }
            public string DeviceName { get; set; }
            public string FriendlyName { get; set; } // Add FriendlyName
            public RECT MonitorRect { get; set; }
            public RECT WorkArea { get; set; }
            public uint Flags { get; set; }
        }

        private List<MonitorInfo> _monitorDetails = new List<MonitorInfo>();

        public List<string> GetConnectedMonitors()
        {
            _monitorDetails.Clear();
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumCallback, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                // TODO: Log the error (Task 8)
                Console.WriteLine($"Error enumerating monitors: {ex.Message}");
                App.ShowNotification("Error", $"Failed to enumerate monitors: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Error); // Show error notification
            }
            // Return friendly names
            return _monitorDetails.Select(m => m.FriendlyName).ToList();
        }

        private bool MonitorEnumCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            try
            {
                if (GetMonitorInfo(hMonitor, ref mi))
                {
                    string deviceName = mi.szDevice.TrimEnd('\0');
                    string friendlyName = deviceName; // Default to device name

                    // Attempt to get a friendly name using EnumDisplayDevices
                    DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
                    dd.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
                    if (EnumDisplayDevices(deviceName, 0, ref dd, 0))
                    {
                        friendlyName = dd.DeviceString.TrimEnd('\0');
                    }
                    else
                    {
                         Console.WriteLine($"Could not get friendly name for device: {deviceName}");
                         // Optionally show a warning notification here
                    }


                    _monitorDetails.Add(new MonitorInfo
                    {
                        HMonitor = hMonitor,
                        DeviceName = deviceName,
                        FriendlyName = friendlyName, // Store friendly name
                        MonitorRect = mi.rcMonitor,
                        WorkArea = mi.rcWork,
                        Flags = mi.dwFlags
                    });
                }
                else
                {
                    // Fallback if getting info fails
                    Console.WriteLine($"Could not get monitor info for handle: {hMonitor}");
                    App.ShowNotification("Warning", $"Could not get monitor info for handle: {hMonitor}", Avalonia.Controls.Notifications.NotificationType.Warning); // Show warning notification
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the error (Task 8)
                Console.WriteLine($"Error getting monitor info: {ex.Message}");
                App.ShowNotification("Warning", $"Could not get monitor info: {ex.Message}", Avalonia.Controls.Notifications.NotificationType.Warning); // Show warning notification
            }

            return true; // Continue enumeration
        }

        // Method to get monitor info by friendly name (or device name as fallback)
        public MonitorInfo? GetMonitorInfoByIdentifier(string identifier)
        {

            if(_monitorDetails.Count == 0)
            {
                GetConnectedMonitors();
            }
            // Try to find by friendly name first
            var monitor = _monitorDetails.FirstOrDefault(m => m.FriendlyName.Equals(identifier, StringComparison.OrdinalIgnoreCase));
            if (monitor.HMonitor != IntPtr.Zero) return monitor;

            // Fallback to device name if friendly name not found
            return _monitorDetails.FirstOrDefault(m => m.DeviceName.Equals(identifier, StringComparison.OrdinalIgnoreCase));
        }


        // TODO: Implement logic for handling display hot-plug (Known Issues in progress.md)
    }
}
