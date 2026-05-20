# KanbanConsole — Project Reference

Use this document when writing stories, acceptance criteria, and implementation code for this project.

---

## Project Summary

A terminal-based Kanban board built with .NET 10 and Spectre.Console. The user navigates menus with arrow keys to add, move, and delete tasks across three columns. Board state is persisted to `kanban.json`.

---

## Solution Structure

```
KanbanConsole/
├── KanbanConsole/          # Console UI — Program.cs (top-level statements)
├── KanbanCore/             # Domain library — models + service logic
└── KanbanCore.Tests/       # NUnit test project for KanbanCore
```

**Key constraint:** `KanbanConsole` (UI) depends on `KanbanCore` (domain). Domain must never depend on UI. Tests cover `KanbanCore` only; UI behavior is exercised manually.

---

## Domain Model (`KanbanCore`)

### `Board` — [KanbanCore/Board.cs](../KanbanCore/Board.cs)

```csharp
public class Board
{
    public List<KanbanTask> Todo       { get; set; } = [];
    public List<KanbanTask> InProgress { get; set; } = [];
    public List<KanbanTask> Done       { get; set; } = [];
    public List<KanbanTask> Blocked    { get; set; } = [];
}
```

### `KanbanTask` — [KanbanCore/Board.cs](../KanbanCore/Board.cs)

```csharp
public record KanbanTask(int Id, string Title);
```

- `Id` is a positive integer, unique across all three columns at any moment.
- `Title` is always trimmed; blank titles are rejected at the UI layer.

### `Column` — [KanbanCore/Board.cs](../KanbanCore/Board.cs)

```csharp
public enum Column { Todo, InProgress, Done, Blocked }
```

---

## Service API (`BoardService`) — [KanbanCore/BoardService.cs](../KanbanCore/BoardService.cs)

All methods are `static`. The `Board` object is mutated in place.

| Method | Signature | Returns | Notes |
|--------|-----------|---------|-------|
| `AddTask` | `(Board, string title, Column)` | `KanbanTask` | Trims title; assigns next unique Id |
| `MoveTask` | `(Board, int id, Column destination)` | `bool` | `false` if id not found or already in destination |
| `DeleteTask` | `(Board, int id)` | `bool` | `false` if id not found |
| `GetAllTasks` | `(Board)` | `List<KanbanTask>` | Todo ++ InProgress ++ Done order |
| `FindTask` | `(Board, int id)` | `(KanbanTask?, List<KanbanTask>)` | Returns task + its backing list |
| `GetTaskColumn` | `(Board, int id)` | `Column?` | `null` if not found |
| `GetColumn` | `(Board, Column)` | `List<KanbanTask>` | Direct reference to the board's list; handles all four columns including `Blocked` |
| `NextId` | `(Board)` | `int` | `max(existing ids) + 1`, or `1` for empty board |

---

## UI Layer (`KanbanConsole`) — [KanbanConsole/Program.cs](../KanbanConsole/Program.cs)

Top-level statements; no class wrapper. Key behaviors:

- Renders the board as a three-column Spectre.Console `Table` on each loop iteration.
- Menu actions: **Add task**, **Move task**, **Delete task**, **Quit**.
- `Save(board)` is called after every mutating action and on Quit.
- `Load()` reads `kanban.json` from the working directory; returns a new `Board` on any failure.
- `ColumnFromString(string)` maps display names (`"To Do"`, `"In Progress"`, `"Done"`, `"Blocked"`) to `Column` enum values. Each case is explicit; the wildcard fallback maps to `Column.Done`.

---

## Test Project (`KanbanCore.Tests`) — [KanbanCore.Tests/BoardServiceTests.cs](../KanbanCore.Tests/BoardServiceTests.cs)

**Framework:** NUnit 4 with `[TestFixture]` / `[Test]` / `[TestCase]`  
**Naming convention:** `MethodName_Condition_ExpectedResult`  
**Arrange pattern:** Always construct a fresh `new Board()` per test — no shared state.

### Existing test coverage

| Subject | Cases covered |
|---------|---------------|
| `AddTask` | Correct column placement (parameterized: Todo, InProgress, Done, Blocked), Id assignment, Id uniqueness, title trimming |
| `MoveTask` | Happy path move, return `true` on success, return `false` for missing id, return `false` when already in destination (including Blocked), no duplication, move to/from Blocked |
| `DeleteTask` | Removes from board, returns `true` on success, returns `false` for missing id |
| `GetAllTasks` | Returns tasks across all columns including Blocked, empty for new board |
| `NextId` | Returns 1 for empty board, returns max+1, board with only Blocked tasks |
| `GetTaskColumn` | Returns correct column (all four including Blocked), returns `null` for missing id |

---

## Conventions to Follow

**Domain changes** go in `KanbanCore`. New behavior requires new `BoardService` methods or model extensions — never embed logic in `Program.cs`.

**Test-first:** Write a failing NUnit test in `BoardServiceTests.cs` before implementing any domain method. Test names must follow `MethodName_Condition_ExpectedResult`.

**Records are immutable:** `KanbanTask` is a `record`. Mutations on a task (e.g., rename) require replacing it in its column list — do not add mutable properties.

**Id stability:** Ids are never reused or resequenced. Once assigned, an Id is permanent for the life of that board.

**Persistence:** Serialization is plain `System.Text.Json` with no custom converters. New model properties must be JSON-serializable primitives or collections.

**No external state in tests:** Tests must not touch the filesystem, read `kanban.json`, or depend on execution order.

---

## Tech Stack

| Concern | Tool |
|---------|------|
| Runtime | .NET 10 |
| Language | C# (latest, nullable enabled, implicit usings) |
| Terminal UI | Spectre.Console |
| Test framework | NUnit 4 |
| Test runner | `dotnet test` |
| Persistence | `kanban.json` via `System.Text.Json` |

---

## Common Commands

```bash
# Run the app
dotnet run --project KanbanConsole

# Run all tests
dotnet test

# Run tests with output
dotnet test --logger "console;verbosity=normal"
```

---

## Changelog

### 2026-05-20 — Blocked Column

**Stories:** `artifacts/story-blocked-status.md` (domain), `artifacts/story-blocked-status-ui.md` (UI)

Added a fourth `Blocked` column to represent tasks that cannot proceed due to an external dependency or impediment.

**Domain (`KanbanCore`):**

- `Column` enum extended with `Blocked`.
- `Board` class gains a `Blocked` property (`List<KanbanTask>`), serialized as part of `kanban.json`.
- `GetAllTasks` now includes `board.Blocked` (Todo → InProgress → Done → Blocked order).
- `FindTask` searches `board.Blocked` alongside the other three lists.
- `GetTaskColumn` returns `Column.Blocked` for tasks residing in that list.
- `GetColumn` handles `Column.Blocked`; the exhaustive switch no longer throws on it.
- 7 new NUnit tests added to `BoardServiceTests.cs` (total: 27). All pass.

**UI (`KanbanConsole/Program.cs`):**

- `RenderBoard` renders a fourth "🚫 Blocked" column (red header). `maxRows` calculation includes `board.Blocked.Count`. Empty-board placeholder row extended to four cells.
- `AddTask` column prompt includes "Blocked" as a selectable destination.
- `MoveTask` destination prompt includes "Blocked". Blocked tasks appear in the task-selection list via `GetAllTasks`.
- `ColumnFromString` has an explicit `"Blocked" => Column.Blocked` case; no silent fallthrough to the wildcard default.
