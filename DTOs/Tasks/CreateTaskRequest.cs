namespace RoutineFlow.DTOs.Tasks;

public class CreateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchieveAction { get; set; } = string.Empty;
}
