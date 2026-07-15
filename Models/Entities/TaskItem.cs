namespace RoutineFlow.Models.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid GoalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchieveAction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Goal Goal { get; set; } = null!;
    public ICollection<TaskCompletion> Completions { get; set; } = new List<TaskCompletion>();
}
