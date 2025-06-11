using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Debug = System.Diagnostics.Debug;

namespace SimpleMonitorTools
{
    public enum NameMatchMode
    {
        Full,
        Contains,
        Regex
    }

    public class ExternalWinUiInteractor
    {

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hwndTrack;
            public uint dwHoverTime;
        }

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private const uint TME_HOVER = 0x00000001;
        private const uint HOVER_DEFAULT = 0xFFFFFFFF;

        private const int INPUT_MOUSE = 0;

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        public static void RealMouseMove(int x, int y)
        {
            // Convert screen coordinates to normalized absolute coordinates (0 to 65535)
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);

            int normalizedX = x * 65535 / screenWidth;
            int normalizedY = y * 65535 / screenHeight;

            var input = new INPUT
            {
                type = INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dx = normalizedX,
                    dy = normalizedY,
                    dwFlags = MOUSEEVENTF_MOVE | 0x8000, // MOUSEEVENTF_ABSOLUTE
                    dwExtraInfo = IntPtr.Zero
                }
            };

            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        public static void MoveAndHover(int x, int y, IntPtr windowHandle)
        {
            SetCursorPos(x, y);

            TRACKMOUSEEVENT tme = new TRACKMOUSEEVENT();
            tme.cbSize = (uint)Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
            tme.dwFlags = TME_HOVER;
            tme.hwndTrack = windowHandle;
            tme.dwHoverTime = HOVER_DEFAULT; // Or a specific time in milliseconds

            TrackMouseEvent(ref tme);
        }

        public static void MoveMouseAndClick(int x, int y)
        {
            SetCursorPos(x, y);
            Thread.Sleep(50);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }


        public ExternalWinUiInteractor() { }
        /// <summary>
        /// Finds and invokes the first control matching the specified criteria in the external WinUI app window.
        /// </summary>
        /// <param name="windowTitle">The title of the external application's main window.</param>
        /// <param name="controlType">The control type to search for (e.g., Button, TextBox).</param>
        /// <param name="nameMatch">The name to match against the control's Name property.</param>
        /// <param name="nameMatchMode">The mode for name matching.</param>
        /// <returns>True if a matching control was found and invoked, false otherwise.</returns>
        public bool InvokeControl(string windowTitle, FlaUI.Core.Definitions.ControlType controlType, string nameMatch, NameMatchMode nameMatchMode = NameMatchMode.Full)
        {
            var processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle) &&
                            p.MainWindowTitle.Contains(windowTitle, StringComparison.OrdinalIgnoreCase));

            foreach (var proc in processes)
            {
                if (InvokeControl(proc, controlType, nameMatch, nameMatchMode))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Finds and invokes the first control matching the specified criteria in the given process's main window.
        /// </summary>
        /// <param name="proc">The process whose main window will be searched.</param>
        /// <param name="controlType">The control type to search for.</param>
        /// <param name="nameMatch">The name to match against the control's Name property.</param>
        /// <param name="nameMatchMode">The mode for name matching.</param>
        /// <returns>True if a matching control was found and invoked, false otherwise.</returns>
        public bool InvokeControl(Process proc, FlaUI.Core.Definitions.ControlType controlType, string nameMatch, NameMatchMode nameMatchMode = NameMatchMode.Full)
        {
            using (var app = Application.Attach(proc))
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                if (window == null)
                    return false;

                window.SetForeground();
                var controls = window.FindAllDescendants(cf => cf.ByControlType(controlType));
                foreach (var control in controls)
                {
                    if (!control.Properties.Name.IsSupported) continue;

                    var name = control.Name;
                    Debug.Print(name);
                    if (IsNameMatch(control, nameMatch, nameMatchMode))
                    {
                        try
                        {
                            if (controlType == ControlType.TreeItem)
                            {
                                ClickParentTreeItem(control);
                            }
                            else
                            {
                                control.Patterns.ScrollItem.Pattern?.ScrollIntoView();

                                var pos = window.BoundingRectangle.North();
                                RealMouseMove(pos.X, pos.Y + 55);
                                control.WaitUntilClickable();
                                control.Click();

                                RealMouseMove(Mouse.Position.X, Mouse.Position.Y + 50);
                            }
                            
                            //control.WaitUntilClickable(TimeSpan.FromSeconds(5));
                            //control.Click();
                            //control.Parent.Click();
                            //control.Parent.Patterns.SelectionItem.Pattern.Select();
                            //control.Parent.Patterns.ExpandCollapse.Pattern.Expand();

                            return true;
                        }
                        catch
                        {
                            // Not invokable, skip
                        }
                    }
                }
            }
            return false;
        }


        public bool ClickParentTreeItem(AutomationElement control)
        {
            var treeItem = control;
            while (treeItem != null && treeItem.ControlType != ControlType.TreeItem)
            {
                treeItem = treeItem.Parent;
            }

            if (treeItem != null && !treeItem.BoundingRectangle.IsEmpty)
            {
                treeItem.Patterns.ScrollItem.Pattern?.ScrollIntoView();
                treeItem.WaitUntilClickable();
                var center = treeItem.BoundingRectangle.Center();
                Mouse.Position = center;
                FlaUI.Core.Input.Mouse.MoveTo(center);
                FlaUI.Core.Input.Mouse.Click();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Checks if the control name matches according to the specified mode.
        /// </summary>
        private bool IsNameMatch(AutomationElement control, string match, NameMatchMode mode)
        {

            string controlName = control.Name;

            if (string.IsNullOrEmpty(controlName) || string.IsNullOrEmpty(match))
                return false;

            bool found = false;
            switch (mode)
            {
                case NameMatchMode.Full:
                    found= string.Equals(controlName, match, StringComparison.OrdinalIgnoreCase);
                    break;
                case NameMatchMode.Contains:
                    found = controlName.IndexOf(match, StringComparison.OrdinalIgnoreCase) >= 0;
                    break;
                case NameMatchMode.Regex:
                    found = Regex.IsMatch(controlName, match, RegexOptions.IgnoreCase);
                    break;
                default:
                    break;
            }

            if (found) return true;

            var cList = control.FindAllDescendants();

            foreach (var c2 in cList)
            {
                if (c2.Properties.Name.IsSupported)
                {
                    return IsNameMatch(c2, match, mode);
                }
            }

            return false;
        }
    }
}
