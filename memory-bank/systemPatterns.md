# System Patterns & Architecture


TrayApp ├── TrayService (NotifyIcon + context menus) ├── ManageWindow (Avalonia modal, MVVM) ├── ShortcutRepository (JSON read/write) ├── MonitorService (P/Invoke EnumDisplayMonitors, cached list) ├── ProcessLauncher (CreateProcess → EnumWindows → SetWindowPos) └── Logging (simple text file debug log)



* **Single-Instance Guard:** mutex in `Program.cs`.
* **MVVM in Avalonia:** `ObservableCollection<ShortcutVM>` bound to `DataGrid`.
* **Repository Pattern:** isolates persistence format from rest of code.
* **Service Locator (via DI container):** keeps tray code slim and testable.