using KanbanCore;

[TestFixture]
public class BoardServiceTests
{
    // ── AddTask ─────────────────────────────────────────────────────────────

    [TestCase(Column.Todo)]
    [TestCase(Column.InProgress)]
    [TestCase(Column.Done)]
    public void AddTask_AddsToCorrectColumn(Column column)
    {
        var board = new Board();
        BoardService.AddTask(board, "Task", column);
        Assert.That(BoardService.GetColumn(board, column), Has.Count.EqualTo(1));
    }

    [Test]
    public void AddTask_ReturnsTaskWithAssignedId()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        Assert.That(task.Id, Is.EqualTo(1));
    }

    [Test]
    public void AddTask_IdsAreUnique()
    {
        var board = new Board();
        var t1 = BoardService.AddTask(board, "First",  Column.Todo);
        var t2 = BoardService.AddTask(board, "Second", Column.InProgress);
        Assert.That(t1.Id, Is.Not.EqualTo(t2.Id));
    }

    [Test]
    public void AddTask_TrimsTitle()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "  trimmed  ", Column.Todo);
        Assert.That(task.Title, Is.EqualTo("trimmed"));
    }

    // ── MoveTask ─────────────────────────────────────────────────────────────

    [Test]
    public void MoveTask_MovesTaskToDestinationColumn()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);

        BoardService.MoveTask(board, task.Id, Column.InProgress);

        Assert.That(board.Todo,       Is.Empty);
        Assert.That(board.InProgress, Has.Count.EqualTo(1));
        Assert.That(BoardService.GetTaskColumn(board, task.Id), Is.EqualTo(Column.InProgress));
    }

    [Test]
    public void MoveTask_ReturnsTrueOnSuccess()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        Assert.That(BoardService.MoveTask(board, task.Id, Column.Done), Is.True);
    }

    [Test]
    public void MoveTask_ReturnsFalseForNonexistentId()
    {
        var board = new Board();
        Assert.That(BoardService.MoveTask(board, 99, Column.Done), Is.False);
    }

    [Test]
    public void MoveTask_ReturnsFalseWhenAlreadyInDestination()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        Assert.That(BoardService.MoveTask(board, task.Id, Column.Todo), Is.False);
    }

    [Test]
    public void MoveTask_DoesNotDuplicateTask()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        BoardService.MoveTask(board, task.Id, Column.Done);
        Assert.That(BoardService.GetAllTasks(board), Has.Count.EqualTo(1));
    }

    // ── DeleteTask ───────────────────────────────────────────────────────────

    [Test]
    public void DeleteTask_RemovesTaskFromBoard()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        BoardService.DeleteTask(board, task.Id);
        Assert.That(BoardService.GetAllTasks(board), Is.Empty);
    }

    [Test]
    public void DeleteTask_ReturnsTrueOnSuccess()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.Todo);
        Assert.That(BoardService.DeleteTask(board, task.Id), Is.True);
    }

    [Test]
    public void DeleteTask_ReturnsFalseForNonexistentId()
    {
        var board = new Board();
        Assert.That(BoardService.DeleteTask(board, 99), Is.False);
    }

    // ── GetAllTasks ──────────────────────────────────────────────────────────

    [Test]
    public void GetAllTasks_ReturnsTasksAcrossAllColumns()
    {
        var board = new Board();
        BoardService.AddTask(board, "A", Column.Todo);
        BoardService.AddTask(board, "B", Column.InProgress);
        BoardService.AddTask(board, "C", Column.Done);
        Assert.That(BoardService.GetAllTasks(board), Has.Count.EqualTo(3));
    }

    [Test]
    public void GetAllTasks_ReturnsEmptyForNewBoard()
    {
        Assert.That(BoardService.GetAllTasks(new Board()), Is.Empty);
    }

    // ── NextId ───────────────────────────────────────────────────────────────

    [Test]
    public void NextId_ReturnsOneForEmptyBoard()
    {
        Assert.That(BoardService.NextId(new Board()), Is.EqualTo(1));
    }

    [Test]
    public void NextId_ReturnsMaxIdPlusOne()
    {
        var board = new Board();
        BoardService.AddTask(board, "A", Column.Todo);
        BoardService.AddTask(board, "B", Column.Done);
        Assert.That(BoardService.NextId(board), Is.EqualTo(3));
    }

    // ── GetTaskColumn ────────────────────────────────────────────────────────

    [Test]
    public void GetTaskColumn_ReturnsCorrectColumn()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.InProgress);
        Assert.That(BoardService.GetTaskColumn(board, task.Id), Is.EqualTo(Column.InProgress));
    }

    [Test]
    public void GetTaskColumn_ReturnsNullForMissingId()
    {
        Assert.That(BoardService.GetTaskColumn(new Board(), 42), Is.Null);
    }

    // ── Blocked column (AC-1 through AC-7) ──────────────────────────────────

    [Test]
    public void AddTask_ToBlockedColumn_TaskAppearsInBlockedList()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Blocked Task", Column.Blocked);
        Assert.That(board.Blocked, Has.Count.EqualTo(1));
        Assert.That(board.Blocked[0].Id, Is.EqualTo(task.Id));
        Assert.That(board.Todo,       Is.Empty);
        Assert.That(board.InProgress, Is.Empty);
        Assert.That(board.Done,       Is.Empty);
    }

    [Test]
    public void MoveTask_FromInProgressToBlocked_TaskMovedSuccessfully()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Task", Column.InProgress);

        var result = BoardService.MoveTask(board, task.Id, Column.Blocked);

        Assert.That(result,           Is.True);
        Assert.That(board.Blocked,    Has.Count.EqualTo(1));
        Assert.That(board.InProgress, Is.Empty);
    }

    [Test]
    public void GetAllTasks_BoardWithBlockedTask_IncludesBlockedTask()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Blocked", Column.Blocked);

        var all = BoardService.GetAllTasks(board);

        Assert.That(all, Has.Count.EqualTo(1));
        Assert.That(all[0].Id, Is.EqualTo(task.Id));
    }

    [Test]
    public void GetTaskColumn_TaskInBlocked_ReturnsBlocked()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Blocked", Column.Blocked);
        Assert.That(BoardService.GetTaskColumn(board, task.Id), Is.EqualTo(Column.Blocked));
    }

    [Test]
    public void MoveTask_AlreadyInBlocked_ReturnsFalse()
    {
        var board = new Board();
        var task = BoardService.AddTask(board, "Blocked", Column.Blocked);

        var result = BoardService.MoveTask(board, task.Id, Column.Blocked);

        Assert.That(result,        Is.False);
        Assert.That(board.Blocked, Has.Count.EqualTo(1));
    }

    [Test]
    public void MoveTask_NonExistentIdToBlocked_ReturnsFalse()
    {
        var board = new Board();

        var result = BoardService.MoveTask(board, 99, Column.Blocked);

        Assert.That(result,        Is.False);
        Assert.That(board.Blocked, Is.Empty);
    }

    [Test]
    public void NextId_BoardWithOnlyBlockedTasks_ReturnsMaxIdPlusOne()
    {
        var board = new Board();
        BoardService.AddTask(board, "First",  Column.Blocked);
        BoardService.AddTask(board, "Second", Column.Blocked);

        Assert.That(BoardService.NextId(board), Is.EqualTo(3));
    }
}
