using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Goals;

public class UpdateGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalIcon Icon { get; set; } = GoalIcon.General;
}
