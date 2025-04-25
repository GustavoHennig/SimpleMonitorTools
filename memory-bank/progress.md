# Progress Log

| Date (2025) | Milestone | Status | Notes |
|-------------|-----------|--------|-------|
| Apr-25 | Repo & solution created | ✅ | Avalonia template + MIT license. |
| Apr-25 | Task 0: Single-instance + tray icon | ⏳ | Tray appears; icon placeholder. |
| — | Task 1: MonitorService | 🟡 | P/Invoke skeleton written, unit tests TBD. |
| — | Task 2: ShortcutRepository | 🔜 | JSON schema defined. |
| — | Task 3: Design Shortcut model + validation helpers | 🔜 | Not started. |
| — | Task 4: Build ManageShortcutsWindow (XAML + ViewModel) | 🟡 | Window created; UI and ViewModel TBD. |
| — | Task 5: Wire tray menu → Manage window, dynamic “Launch” submenu | 🟡 | "Manage Shortcuts" menu item wired; dynamic "Launch" submenu TBD. |
| — | Task 6: Implement ProcessLauncher | 🔜 | Not started. |
| — | Task 7: Glue: Add / remove shortcut flow, auto-save, update tray submenu | 🔜 | Not started. |
| — | Task 8: Error handling & toast notifications | 🔜 | Not started. |
| — | Task 9: Optional “Run on startup” toggle & registry helper | 🔜 | Not started. |
| — | Task 10: Polishing | 🔜 | Not started. |
| — | Task 11: Build pipeline | 🔜 | Not started. |
| — | Task 12: Smoke testing | 🔜 | Not started. |

### What Works
* Application launches to tray; exit menu functional.
* "Manage Shortcuts..." menu item in tray opens a window.

### What’s Left
* Implement the UI and ViewModel for the Manage Shortcuts window.
* Implement the dynamic "Launch" submenu in the tray icon's context menu.
* Implement MonitorService (Task 1).
* Implement ShortcutRepository (Task 2).
* Design Shortcut model and validation helpers (Task 3).
* Implement ProcessLauncher (Task 6).
* Implement the add/remove shortcut flow, auto-save, and update tray submenu logic (Task 7).
* Implement error handling and toast notifications (Task 8).
* Implement optional "Run on startup" toggle (Task 9).
* Polishing, build pipeline, and testing (Tasks 10-12).

### Known Issues
* Tray icon disappears if Explorer restarts (needs GUID + re-init handler).
* Monitor enumeration not yet refreshed on display hot-plug.

### Evolution Notes
* May add Avalonia.Notifications for toast messages after MVP is solid.
