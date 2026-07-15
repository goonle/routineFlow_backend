using RoutineFlow.Models.Enums;

namespace RoutineFlow.Models.Entities;

public class Plan
{
    public Guid Id { get; set; }
    public Guid GoalId { get; set; }
    public PlanType Type { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int RepeatCount { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Goal Goal { get; set; } = null!;
}
