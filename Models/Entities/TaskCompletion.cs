namespace RoutineFlow.Models.Entities;

public class TaskCompletion
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public DateOnly Date { get; set; }
    public int Count { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public TaskItem Task { get; set; } = null!;
}
