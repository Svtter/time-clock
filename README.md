# TimeClock

A lightweight, always-on-top desktop analog clock for Windows, built with WPF (.NET 9).

![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)

[中文文档](README.zh-CN.md)

## Features

- **Always on top** — transparent, borderless floating clock window
- **Analog clock face** — 12 hour numbers, 60 tick marks, hour/minute/second hands
- **Digital time display** — shown below the clock face
- **Date display** — current date and day of week
- **Draggable** — click and drag to reposition; position is remembered
- **System tray** — minimizes to tray with right-click context menu
- **Auto-start** — option to launch on Windows startup (via registry)
- **Toggle second hand** — show or hide the second hand in settings
- **Logging** — automatic logging of app lifecycle events, stored in `%APPDATA%/TimeClock/Logs/`

## Requirements

- Windows 10/11
- .NET 9 SDK

## Build & Run

```bash
dotnet build TimeClock
dotnet run --project TimeClock
```

## Usage

- **Move** — left-click and drag the clock
- **Settings** — right-click → "Settings..."
- **View logs** — right-click → "View Logs"
- **Exit** — right-click → "Exit"

## Project Structure

```
TimeClock/
├── Helpers/
│   ├── Logger.cs              # File-based logging
│   └── SettingsManager.cs     # Settings persistence & auto-start
├── App.xaml / .cs             # Entry point, tray icon
├── MainWindow.xaml / .cs      # Floating clock window
├── SettingsWindow.xaml / .cs  # Settings dialog
├── LogWindow.xaml / .cs       # Log viewer
└── GlobalUsings.cs
```

## License

Licensed under the [Apache License 2.0](LICENSE).
