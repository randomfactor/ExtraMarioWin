Project: Extra Mario (WPF, .NET 9)

Goal
- Manage a karaoke roster efficiently with a simple WPF UI (Next, Bump, Add, Remove, Reorder) and lightweight, daily CSV persistence.

Context
- Platform: WPF on .NET 9
- Domain: Karaoke show roster management
- Mario is a great and famous Karaoke Jockey; this app helps manage the roster like having an extra Mario on hand.

Core Requirements
1) Roster UI/Behavior
- Show a list of singers (stage names). Long names ellipsize, remove icon on the right, edit (pencil) next to it, and a grab handle (?) on the left for dragging.
- Buttons on top bar: `Next`, `Bump`, `Add`.
- `Next`: advance the current singer to the end (index 0 -> end). No-op returns false if 0 singers; returns true if 1 or more. Before advancing, log the just-finished singer to the history file. After advancing, scroll the ListBox so the new current singer (top) is visible.
- `Bump`: swap the first two singers if there are at least two; otherwise no-op.
- `Add`: opens a small dialog (focus inside the TextBox); adds new singer with a new `Guid`.
- `Remove`: confirm and remove a singer.
- `Reorder`: drag a singer by the grab handle and drop at a different position. Only single-item moves supported. Persist new order.
- `Edit`: click the pencil icon to change a singer’s stage name. Guid must be retained.
- Duplicate-name prevention: Add/Edit dialogs disable OK and show a red warning if the name is empty or already exists (case-insensitive) in the current roster.

2) Data Model
- `KSinger`: fields `id` (Guid) and `stageName` (string). Exposes CLR properties `Id` and `StageName` (INotifyPropertyChanged), and a computed `StageBrush` (deterministic pastel color from `Id`).
- `KRoster`: in-memory roster with methods `Add`, `Remove`, `Get`, `Count`, `Bump`, `NextSinger` (and `Next` wrapper), and `Move(oldIndex,newIndex)`.

3) History Logging (Daily CSV)
- Interface: `IPerformerHistory` with method `SaveSinger(KSinger singer)`.
- Default implementation: `FilePerformerHistory`.
- File location: `%LocalAppData%/Extra Mario/`.
- File name: `history_YYYY-MM-DD.csv` (today’s date).
- Row format: "Singer Name","YYYY-MM-DDTHH:mm:ss±hh:mm" (ISO 8601 without fractional seconds; CSV-escaped name).
- Flush on each append.
- `FilePerformerHistory` must be testable via optional constructor base directory override.

4) Roster Persistence (Daily CSV)
- Interface: `IPerformerRosterStorage` with methods `SaveRoster(IReadOnlyList<KSinger>)` and `RestoreRoster(): List<KSinger>`.
- Default implementation: `FilePerformerRosterStorage`.
- File location: same directory as history: `%LocalAppData%/Extra Mario/`.
- File name: `roster_YYYY-MM-DD.csv` (today’s date).
- Row format: `Guid,"Singer Name"` (CSV-escaped name). `SaveRoster` overwrites the file each call.
- On app startup: if today’s roster file exists, restore it; otherwise start empty.
- `FilePerformerRosterStorage` must be testable via optional constructor base directory override.

5) UI/UX Notes
- Initial window size: 600x600; 3px gray (#808080) outer margin (window background provides the gray).
- Button bar has a bright-pink-to-deep-purple diagonal gradient and raised button style with pressed visual.
- Singer items have pastel backgrounds determined by `KSinger.Id`, compact margin/padding to show ~10 singers, and include grab, edit, and remove controls.
- The ListBox gradient background should be visible even when the list is empty.

6) Dependency Injection
- `MainWindow` constructor takes `IPerformerHistory` and `IPerformerRosterStorage` with default implementations `FilePerformerHistory` and `FilePerformerRosterStorage` used by the parameterless constructor.

7) Tests
- Unit tests exist for `KRoster` behaviors (Add, Remove, Get, Bump, Next, Move) and for file persistence classes.
- `FilePerformerHistory` and `FilePerformerRosterStorage` tests use temporary folders via constructor overrides.

Acceptance Checklist
- Next/Bump/Add/Remove/Edit/Reorder behaviors function correctly.
- Text input dialogs focus the TextBox on open; Enter confirms; Esc cancels; OK disabled and warning shown for duplicate names.
- History file appends a line with singer name and timestamp each time Next is clicked, before roster advances.
- Roster file overwrites on save and restores on app startup; reorders persist.
- All timestamps in history exclude fractional seconds.
- UI renders with modern button bar, gradient backgrounds, compact per-singer layout, and stable per-singer pastel colors.
- All unit tests pass (`dotnet test`).

Out of Scope
- Multi-show/multi-day management beyond the daily files described.
- Database/cloud sync; out of scope for now but pluggable via interfaces.

How to Run
- Build: `dotnet build`
- Run app: `dotnet run --project ExtraMarioWin`
- Tests: `dotnet test`
