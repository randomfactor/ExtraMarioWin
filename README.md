# Extra Mario (WPF, .NET 9)

A lightweight WPF app to manage a karaoke singer roster quickly and cleanly — like having an extra Mario.

Mario is a great and famous Karaoke Jockey (KJ). This application helps manage the roster like having an extra Mario on hand.

## Features
- Add, remove, bump, and advance (`Next`) singers
- Drag-and-drop reordering with grab handles (?) on each singer
- Edit singer names via the pencil button; Guid is retained
- Duplicate-name protection: Add/Edit dialogs disable OK and show a warning until the name is unique
- Modernized UI with readable list and per-singer pastel colors
- Daily history logging: appends the finished singer and timestamp to `history_YYYY-MM-DD.csv`
- Daily roster persistence: saves and restores `roster_YYYY-MM-DD.csv` on app start
- Pluggable persistence via `IPerformerHistory`/`FilePerformerHistory` and `IPerformerRosterStorage`/`FilePerformerRosterStorage`

## Build and Run
Requirements: .NET SDK 9.0

- Build: `dotnet build`
- Run (from solution directory): `dotnet run --project ExtraMarioWin`
- Tests: `dotnet test`

## Where files are stored
Files are written under the user LocalApplicationData folder in `Extra Mario`:
- History: `history_YYYY-MM-DD.csv` ("Singer Name","YYYY-MM-DDTHH:mm:ss±hh:mm")
- Roster: `roster_YYYY-MM-DD.csv` (per line: `Guid,"Singer Name"`)

On startup the app attempts to restore today’s roster; if the file does not exist, the roster starts empty.

## Architecture Snapshot
- `KSinger`: model for a singer (Id, StageName, StageBrush)
- `KRoster`: in-memory roster management (Add, Remove, Bump, Next, Move)
- `IPerformerHistory` / `FilePerformerHistory`: append-only daily history
- `IPerformerRosterStorage` / `FilePerformerRosterStorage`: daily roster save/restore
- `MainWindow`: WPF UI and simple event handlers

## Extending
Swap `FilePerformerHistory` and `FilePerformerRosterStorage` with your own implementations (e.g., database, cloud) by implementing the respective interfaces.
