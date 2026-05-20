namespace KanbanCore;

public record KanbanTask(int Id, string Title);

public enum Column { Todo, InProgress, Done, Blocked }

public class Board
{
    public List<KanbanTask> Todo       { get; set; } = [];
    public List<KanbanTask> InProgress { get; set; } = [];
    public List<KanbanTask> Done       { get; set; } = [];
    public List<KanbanTask> Blocked    { get; set; } = [];
}
