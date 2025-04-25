# Simple Monitor Tool

### 1. Product Overview
**Simple Monitor Tool** is a lightweight Windows utility, written with **Avalonia UI**, that lives exclusively in the system-tray. Its sole purpose is to let a user register one-click shortcuts to executable files and automatically launch each executable on a specific display/monitor.

---

### 2. Scope & Out-of-Scope
|                       | Included (MVP) | Excluded (later / never) |
|-----------------------|----------------|--------------------------|
| System-tray icon with context menu        | ✅ | — |
| “Manage Shortcuts” dialog (modal window)  | ✅ | Full main window, task-bar presence |
| Add / remove shortcut entries             | ✅ | Edit in-place name/path/monitor *(may come later)* |
| Launch shortcut from tray submenu         | ✅ | Global hotkeys, drag-and-drop launcher |
| Persist configuration as JSON under %APPDATA% | ✅ | Cloud sync, registry storage |
| Detect & enumerate all active monitors    | ✅ | Per-monitor DPI scaling tweaks |
| Start chosen executable on desired monitor (no position fine-tuning) | ✅ | Window-size/position presets |
| Auto-start the utility with Windows *(optional toggle)* | ✅ | Installer, auto-update logic |

---

### 3. Functional Requirements

1. **Tray Lifecycle**
   * On application start, create a single **NotifyIcon** with:
     * App logo.
     * Left-click ➜ show “Manage Shortcuts” dialog.
     * Right-click ➜ context menu:
       * Submenu **Launch** → a dynamic list of saved shortcuts.
       * **Manage Shortcuts…**
       * **Reload Monitors** (refresh enumeration without restart).
       * **Exit**.

2. **Manage Shortcuts Dialog**
   * Modal window (no task-bar entry) with an `ItemsControl` showing rows:  
     `[Executable Path] [Target Monitor] [Remove Button]`
   * **Add…** button opens a small panel/inline editor:
     * `TextBox` (path) + file-picker button.
     * `ComboBox` (monitor list, resolved names like “\\.\DISPLAY2 (1920×1080)”).
     * **Save / Cancel** buttons.
   * Validation:
     * Executable path must exist and end in `.exe`.
     * Monitor selection cannot be empty.
     * Duplicate paths allowed (user can assign same exe to multiple monitors).

3. **Monitor Enumeration**
   * At launch and on explicit “Reload Monitors”:
     * Use **Windows API** (`EnumDisplayMonitors` or `User32.GetMonitorInfo`) via P/Invoke to list active, *attached* displays.
     * Store monitor identifier (`DeviceName` such as `\\.\DISPLAY1`) and its bounds (`RECT`).

4. **Launching Logic**
   * When user selects a shortcut:
     1. Spawn the process with `CreateProcess`.  
     2. Immediately after process handle is valid, wait for the first top-level window (`EnumWindows + GetWindowThreadProcessId`).
     3. Move the window to the target monitor’s working area using `SetWindowPos` *(simple top-left anchor only)*.

5. **Persistence**
   * File: `%APPDATA%\SimpleMonitorTool\shortcuts.json`
   * Structure:
     ```json
     {
       "version": 1,
       "shortcuts": [
         { "path": "C:\\Tools\\MyApp.exe", "monitorId": "\\\\.\\DISPLAY2" }
       ]
     }
     ```
   * Load on startup; save immediately after any change.

6. **Startup Behaviour**
   * If `settings.json` (future) has `"RunOnStartup": true`, add a key in  
     `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.

---

### 4. Non-Functional Requirements

| Category          | Requirement |
|-------------------|-------------|
| **Performance**   | App idle memory ≤ 30 MB; launch shortcut overhead ≤ 100 ms before handing control to target exe. |
| **Reliability**   | Gracefully handles missing executables or disconnected monitors (show toast + log). |
| **Portability**   | Built with Avalonia UI but only Windows APIs are required in MVP (Linux/macOS UI can compile but launching logic can be stubbed). |
| **Usability**     | No task-bar clutter; all dialogs close when focus lost (optional). |
| **Security**      | No elevation needed; writes only to user profile. |
| **Logging**       | Minimal text log in `%APPDATA%\SimpleMonitorTool\log.txt` (debug level behind compile-time flag). |

---

### 5. High-Level Architecture

```text
┌──────────────────┐
│ Program.cs       │ → single-instance guard
└──────────────────┘
        │
        ▼
┌──────────────────┐
│ TrayService      │
│ · Build menus    │
│ · Handle clicks  │
└──────────────────┘
        │
        ├───► MonitorService (enumerate, cache)
        │
        ├───► ShortcutRepository (load/save JSON)
        │
        └───► ProcessLauncher (spawn + move window)
```

*MVVM pattern* for the Manage Shortcuts window; `ObservableCollection<ShortcutViewModel>` bound to a simple `DataGrid` or `ItemsRepeater`.

---

### 6. Implementation Task Breakdown

| # | Task | Est. |
|---|------|------|
| **0** | Project bootstrapping: Avalonia template, single-instance mutex, basic tray icon. | 0.5 d |
| **1** | Implement **MonitorService** with P/Invoke; expose `List<MonitorInfo>`. | 1 d |
| **2** | Implement **ShortcutRepository** (read/write JSON, create folder). | 0.5 d |
| **3** | Design **Shortcut** model + validation helpers. | 0.5 d |
| **4** | Build **ManageShortcutsWindow** (XAML + ViewModel). | 1 d |
| **5** | Wire tray menu → Manage window, dynamic “Launch” submenu. | 0.5 d |
| **6** | Implement **ProcessLauncher**: spawn exe, wait for window, move to monitor. | 1 d |
| **7** | Glue: Add / remove shortcut flow, auto-save, update tray submenu. | 0.5 d |
| **8** | Error handling & toast notifications (Avalonia.Notifications). | 0.5 d |
| **9** | Optional “Run on startup” toggle & registry helper. | 0.5 d |
| **10** | Polishing: icons, file dialogs, keyboard access, DPI tests. | 0.5 d |
| **11** | Build pipeline: Release config, self-contained publish, code signing stub. | 0.5 d |
| **12** | Smoke testing on single/dual/quad-monitor setups; fix edge cases. | 1 d |

**Total**: ~8 developer-days (single dev).

---

### 7. Deliverables

1. **Source Code** (MIT or chosen license).
2. `README.md` with build/run instructions.
3. Signed **Setup.exe** *(or ZIP)*.
4. Sample `shortcuts.json`.
5. Changelog v0.1.0.

---

### 8. Future-Ready Notes (not implemented now)

* Per-shortcut custom icon in tray.
* Window size/position templates.
* Hotkey to cycle monitors for current foreground window.

*(Listed only for roadmap awareness; **not** part of current specification.)*

---

This document fully specifies the MVP for **Simple Monitor Tool** and outlines a practical task list to build it without introducing any non-essential features.