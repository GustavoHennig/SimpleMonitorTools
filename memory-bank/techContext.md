# Tech Context

| Area | Choice | Notes |
|------|--------|-------|
| UI           | **Avalonia UI 11** | Cross-platform; Windows is MVP target. |
| Framework    | **.NET 8**        | Self-contained publish. |
| Language     | **C# 12**         | Latest LTS features. |
| Win32 Access | `User32.dll` P/Invoke | Monitor enumeration & window movement. |
| Data Store   | JSON via `System.Text.Json` | `%APPDATA%\SimpleMonitorTool`. |
| DI           | Out of scope |
| Logging      | Print.Debut only | Only active in DEBUG builds. |
| Build Tool   | `dotnet publish -c Release -p:PublishSingleFile=true` | Generates a single EXE. |

**Constraints**

* Works on Windows 10/11 only (monitor APIs unavailable on Linux/macOS for MVP).
* Must not require elevation.
* Idle CPU use ~0 %.
