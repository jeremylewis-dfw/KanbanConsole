using System.Text.Json;
using KanbanCore;
using Spectre.Console;

const string DataFile = "kanban.json";

var board = Load();

while (true)
{
    Console.Clear();
    RenderBoard(board);

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\n[bold]What would you like to do?[/]")
            .AddChoices("Add task", "Move task", "Delete task", "Quit"));

    switch (choice)
    {
        case "Add task":    AddTask(board);    break;
        case "Move task":   MoveTask(board);   break;
        case "Delete task": DeleteTask(board); break;
        case "Quit":        Save(board); return;
    }

    Save(board);
}

static void RenderBoard(Board board)
{
    var table = new Table().Border(TableBorder.Rounded).Expand();
    table.AddColumn(new TableColumn("[yellow bold]📋 To Do[/]").Centered());
    table.AddColumn(new TableColumn("[blue bold]🔄 In Progress[/]").Centered());
    table.AddColumn(new TableColumn("[green bold]✅ Done[/]").Centered());

    int maxRows = Math.Max(board.Todo.Count, Math.Max(board.InProgress.Count, board.Done.Count));

    if (maxRows == 0)
    {
        table.AddRow("[dim]—[/]", "[dim]—[/]", "[dim]—[/]");
    }
    else
    {
        for (int i = 0; i < maxRows; i++)
        {
            string todo = i < board.Todo.Count       ? $"[yellow]{board.Todo[i].Id}[/]. {Markup.Escape(board.Todo[i].Title)}"           : "";
            string wip  = i < board.InProgress.Count ? $"[blue]{board.InProgress[i].Id}[/]. {Markup.Escape(board.InProgress[i].Title)}" : "";
            string done = i < board.Done.Count       ? $"[green]{board.Done[i].Id}[/]. {Markup.Escape(board.Done[i].Title)}"            : "";
            table.AddRow(todo, wip, done);
        }
    }

    AnsiConsole.Write(new Rule("[bold]Kanban Board[/]").RuleStyle("grey"));
    AnsiConsole.Write(table);
}

static void AddTask(Board board)
{
    var title = AnsiConsole.Ask<string>("Task title:");
    if (string.IsNullOrWhiteSpace(title)) return;

    var column = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Add to column:")
            .AddChoices("To Do", "In Progress", "Done"));

    BoardService.AddTask(board, title, ColumnFromString(column));
}

static void MoveTask(Board board)
{
    var all = BoardService.GetAllTasks(board);
    if (all.Count == 0) { AnsiConsole.MarkupLine("[red]No tasks to move.[/]"); Pause(); return; }

    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select task to move:")
            .AddChoices(all.Select(t => $"{t.Id}. {t.Title}")));

    int id = int.Parse(selected.Split('.')[0]);

    var destination = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Move to:")
            .AddChoices("To Do", "In Progress", "Done"));

    BoardService.MoveTask(board, id, ColumnFromString(destination));
}

static void DeleteTask(Board board)
{
    var all = BoardService.GetAllTasks(board);
    if (all.Count == 0) { AnsiConsole.MarkupLine("[red]No tasks to delete.[/]"); Pause(); return; }

    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select task to delete:")
            .AddChoices(all.Select(t => $"{t.Id}. {t.Title}")));

    int id = int.Parse(selected.Split('.')[0]);
    BoardService.DeleteTask(board, id);
}

static Column ColumnFromString(string name) => name switch
{
    "To Do"       => Column.Todo,
    "In Progress" => Column.InProgress,
    _             => Column.Done,
};

static void Pause()
{
    AnsiConsole.MarkupLine("[dim]Press any key...[/]");
    Console.ReadKey(true);
}

static Board Load()
{
    if (!File.Exists(DataFile)) return new Board();
    try
    {
        var json = File.ReadAllText(DataFile);
        return JsonSerializer.Deserialize<Board>(json) ?? new Board();
    }
    catch { return new Board(); }
}

static void Save(Board board)
{
    var json = JsonSerializer.Serialize(board, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(DataFile, json);
}
