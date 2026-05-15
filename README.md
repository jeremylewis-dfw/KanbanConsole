# KanbanConsole

A simple terminal-based Kanban board built with .NET 10 and [Spectre.Console](https://spectreconsole.net/).

## Requirements

- [.NET 10 SDK](https://dot.net/download) or later

## Running the app

```bash
dotnet run --project KanbanConsole
```

Board state is saved to `kanban.json` in the working directory.

## Data file

Board state is persisted to `kanban.json` using a relative path, so the file is created in whatever directory you run the command from. For example, running from the solution root:

```bash
cd /Users/jeremy.lewis/Projects/EWN/KanbanConsole
dotnet run --project KanbanConsole
```

saves the file to `/Users/jeremy.lewis/Projects/EWN/KanbanConsole/kanban.json`. If you run from a different directory, the file will be created there instead.

## Usage

Use the arrow keys to navigate menus and Enter to select.

| Action | Description |
|--------|-------------|
| Add task | Create a new task and place it in any column |
| Move task | Move an existing task between columns |
| Delete task | Remove a task from the board |
| Quit | Save and exit |

## Project structure

```
KanbanConsole/
├── KanbanConsole.sln
├── KanbanConsole/
│   ├── KanbanConsole.csproj
│   └── Program.cs              # UI loop
├── KanbanCore/
│   ├── KanbanCore.csproj
│   ├── Board.cs                # KanbanTask, Board, Column models
│   └── BoardService.cs         # Add, Move, Delete, Find logic
└── KanbanCore.Tests/
    ├── KanbanCore.Tests.csproj
    └── BoardServiceTests.cs    # NUnit tests for BoardService
```

## Running tests

```bash
dotnet test
```
