# Progress Log

| Date (2025) | Milestone | Status | Notes |
|-------------|-----------|--------|-------|
| Apr-25 | Repo & solution created | ✅ | Avalonia template + MIT license. |
| Apr-25 | Task 0: Single-instance + tray icon | ⏳ | Tray appears; icon placeholder. |
| Apr-25 | Task 1: MonitorService | ✅ | P/Invoke skeleton written. |
| Apr-25 | Task 2: ShortcutRepository | ✅ | JSON schema defined; Load/Save implemented. |
| Apr-25 | Task 3: Design Shortcut model + validation helpers | ✅ | Model class with basic validation implemented. |
| Apr-25 | Task 4: Build ManageShortcutsWindow (XAML + ViewModel) | ✅ | UI and ViewModel implemented. |
| Apr-25 | Task 5: Wire tray menu → Manage window, dynamic “Launch” submenu | ✅ | "Manage Shortcuts" menu item wired; dynamic "Launch" submenu implemented. |
| Apr-25 | Task 6: Implement ProcessLauncher | ✅ | Process launching and window positioning implemented. |
| Apr-25 | Task 7: Glue: Add / remove shortcut flow, auto-save, update tray submenu | ✅ | Add/remove flow, auto-save, and tray menu updates implemented. |
| — | Task 8: Error handling & toast notifications | 🔜 | Not started. |
| — | Task 9: Optional “Run on startup” toggle & registry helper | 🔜 | Not started. |
| — | Task 10: Polishing | 🔜 | Not started. |
| — | Task 11: Build pipeline | 🔜 | Not started. |
| — | Task 12: Smoke testing | 🔜 | Not started. |

### What Works
* Application launches to tray; exit menu functional.
* "Manage Shortcuts..." menu item in tray opens a window.
* MonitorService implemented (Task 1).
* ShortcutRepository implemented (Task 2).
* Shortcut model and validation helpers designed (Task 3).
* ManageShortcutsWindow built (Task 4).
* Tray menu wired, including dynamic "Launch" submenu (Task 5).
* ProcessLauncher implemented (Task 6).
* Add/remove shortcut flow, auto-save, and tray menu updates implemented (Task 7).

### What’s Left
* Task 0: Single-instance + tray icon (Tray appears; icon placeholder).
* Implement error handling & toast notifications (Task 8).
* Implement optional “Run on startup” toggle (Task 9).
* Polishing (Task 10).
* Build pipeline (Task 11).
* Smoke testing (Task 12).

### Known Issues
* Tray icon disappears if Explorer restarts (needs GUID + re-init handler).
* Monitor enumeration not yet refreshed on display hot-plug.

### Evolution Notes
* May add Avalonia.Notifications for toast messages after MVP is solid.
