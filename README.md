# Simple Monitor Tool

Simple Monitor Tool is a tiny Windows tray app that makes life easier when you use more than one monitor.

---

## What It Does

- **Create shortcuts** that open a program directly on the monitor you choose.
- **One‑click launch** from the tray menu—no more dragging windows around.
- **Remembers your setup** after reboot (settings saved in your user profile).

---

## Why Use It?

If you regularly open apps on the “wrong” screen and have to move them every time.
Simple Monitor Tool makes it automatic — just click, and the app appears on the right monitor.


---

## Quick Start

1. **Download & run** `SimpleMonitorTool.exe` (no installer needed, Windows 10/11).
2. Right‑click the new tray icon → **Manage Shortcuts…**
3. Click **Add**, choose the executable and the target monitor.
4. From now on, start the program via the tray submenu and it will appear on that monitor.

> **Tip:** Add the app to **Start‑up** (tray → Settings → Run on Windows start‑up) so it’s always ready.

---

## How It Works ( under the hood )

1. Add shortcuts using a simple window (opened from the tray).
2. Pick which monitor each shortcut should use.
3. Click the shortcut from the tray menu anytime you want to launch it.
4. Done! No more dragging windows around.


### Under the hood
- Detects connected monitors via Win32 APIs.
- Launches your program, waits for its first window, then moves the window to the chosen screen.
- Stores shortcut data in `%APPDATA%\SimpleMonitorTool\shortcuts.json` so nothing touches the registry.

---

## Requirements

- Windows 10 or 11 (x64)
- .NET 8 Runtime (bundled in the single‑file build)

---

## Roadmap

| Planned Feature | Status |
|-----------------|--------|
| Tray icon tool‑tips & toast errors | 🔜 |
| “Run on start‑up” toggle | 🔜 |
| Hot‑keys to launch shortcuts | 📝 idea |
| Auto‑detect new monitors | 📝 idea |

---

## Technology

Built with **NET 9** and **Avalonia UI**.

---

## Screenshots

![Tray Icon](screenshot-tray.png)
![Manage Shortcuts Window](screenshot-settings.png)

---

## License

MIT – do what you want, just don’t blame us if something breaks.


## Transparency

This is an AI-assisted project, including this README.

Just venting — no AI can write decent Avalonia UI code yet, I had to fix every single attempt by hand.


by Gustavo Hennig
