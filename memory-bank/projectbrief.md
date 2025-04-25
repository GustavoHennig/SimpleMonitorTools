# Simple Monitor Tool – Project Brief

**Goal:**  
Create a lightweight Windows tray utility that launches any registered executable on a user-chosen monitor.

**Success Criteria**

1. **Usability** – One–click launch from tray; zero configuration friction.
2. **Accuracy** – Always positions window on the monitor configured for that shortcut.
3. **Reliability** – Survives monitor changes, missing executables, and restarts without data loss.
4. **Low-impact** – < 30 MB RAM at idle, no task-bar entry, runs under standard user privileges.

**MVP Scope**

* Tray icon with context menu.
* Modal “Manage Shortcuts” dialog reachable from tray.
* Add / remove shortcut entries (exe + monitor).
* Persist configuration to `%APPDATA%\SimpleMonitorTool\shortcuts.json`.
* Enumerate active monitors at runtime.
* Spawn process and move its first window to the correct monitor.

**Out-of-scope (future)**  
Hot-keys, window-size presets, cross-platform monitor handling, auto-update.
