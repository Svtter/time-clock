# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.1] - 2026-08-29

### Changed

- Second hand now sweeps smoothly instead of jumping once per second; hands are redrawn every frame so a busy UI thread no longer makes the tick rhythm irregular

## [0.3.0] - 2026-08-27

### Added

- Alarm support with shake animation: the clock shakes for 15 seconds when an alarm fires; click the clock to stop it early
- Settings dialog reorganized into tabs (General / Alarms)
- Multiple alarms with time (HH:mm) and optional label
- Automatic migration from the legacy single-alarm setting
- Adaptive text color: the digital time and date switch between light and dark based on the background behind the clock
- WiX MSI installer for win-x64 (framework-dependent and self-contained variants)
- Screenshot in README (English and Chinese)

## [0.1.0] - 2026-05-18

### Added

- Always-on-top transparent floating analog clock window
- Hour, minute, and second hands with smooth rotation
- 12 hour numbers and 60 tick marks on the clock face
- Digital time display below the clock face
- Date and day of week display
- Draggable window with position persistence
- System tray icon with right-click context menu
- Settings dialog with auto-start on boot option
- Toggle second hand visibility in settings
- File-based logging with built-in log viewer
- Window position saved across sessions
- Apache 2.0 license
- Bilingual README (English / Chinese)
- GitHub Actions CI and release workflows
