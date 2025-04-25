# Active Context (25 Apr 2025)

### Current Focus
Implement MVP task list (see progress.md).
Immediate work on **Task 0, 1, 4, and 5**: bootstrapping, monitor enumeration, building the ManageShortcutsWindow, and wiring the tray menu.

### Recent Decisions
* Application will be a system tray application with no main window in the MVP.
* The "Manage Shortcuts" window will be opened from the tray icon's context menu.
* Initially showing the "Manage Shortcuts" window as non-modal using `Show()` due to the absence of a main owner window. Modal behavior will be addressed later if needed.
* Use **`SetWindowPos`** after window creation; no DPI-aware resize in MVP.
* Store monitor identifier as `DeviceName (e.g., \\.\DISPLAY2)` – survives reboot.
* No editing in grid for now; delete & re-add is adequate.

### Next Steps
1. Continue implementing **Task 1: MonitorService** with P/Invoke and unit tests.
2. **Task 4: ManageShortcutsWindow** (XAML UI and ViewModel) is implemented.
3. Continue wiring **Task 5: Wire tray menu** (implement the dynamic "Launch" submenu).
4. Implement **Task 2: ShortcutRepository** (read/write JSON, create folder).
5. Implement **Task 3: Design Shortcut model + validation helpers**.
6. Implement **Task 6: ProcessLauncher**.

### Important Patterns / Preferences
* **MVVM-light** (no third-party frameworks).
* **Async everywhere** – no blocking UI thread, use `Task.Run` for Win32 polling.
* All dialogs **WindowStartupLocation = CenterOwner**.

### Insights
Win32 window discovery can race; moving window after a short loop (max 1 s, 50 ms intervals) is simpler than WH_CBT hook.
