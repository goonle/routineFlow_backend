namespace RoutineFlow.DTOs.Tasks;

public class TaskResponse
{
    public Guid Id { get; set; }
    public Guid GoalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchieveAction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
