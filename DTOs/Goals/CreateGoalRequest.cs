namespace RoutineFlow.DTOs.Goals;

public class CreateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
