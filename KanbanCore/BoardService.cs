namespace KanbanCore;

public static class BoardService
{
    public static KanbanTask AddTask(Board board, string title, Column column)
    {
        var task = new KanbanTask(NextId(board), title.Trim());
        GetColumn(board, column).Add(task);
        return task;
    }

    public static bool MoveTask(Board board, int id, Column destination)
    {
        var (task, source) = FindTask(board, id);
        if (task is null) return false;

        var dest = GetColumn(board, destination);
        if (ReferenceEquals(source, dest)) return false;

        source.Remove(task);
        dest.Add(task);
        return true;
    }

    public static bool DeleteTask(Board board, int id)
    {
        var (task, source) = FindTask(board, id);
        if (task is null) return false;
        source.Remove(task);
        return true;
    }

    public static List<KanbanTask> GetAllTasks(Board board) =>
        [.. board.Todo, .. board.InProgress, .. board.Done];

    public static (KanbanTask? Task, List<KanbanTask> Column) FindTask(Board board, int id)
    {
        foreach (var col in new[] { board.Todo, board.InProgress, board.Done })
        {
            var t = col.FirstOrDefault(x => x.Id == id);
            if (t is not null) return (t, col);
        }
        return (null, board.Todo);
    }

    public static Column? GetTaskColumn(Board board, int id)
    {
        if (board.Todo.Any(t => t.Id == id))       return Column.Todo;
        if (board.InProgress.Any(t => t.Id == id)) return Column.InProgress;
        if (board.Done.Any(t => t.Id == id))       return Column.Done;
        return null;
    }

    public static List<KanbanTask> GetColumn(Board board, Column column) => column switch
    {
        Column.Todo       => board.Todo,
        Column.InProgress => board.InProgress,
        Column.Done       => board.Done,
        _ => throw new ArgumentOutOfRangeException(nameof(column))
    };

    public static int NextId(Board board)
    {
        var all = GetAllTasks(board);
        return all.Count > 0 ? all.Max(t => t.Id) + 1 : 1;
    }
}
