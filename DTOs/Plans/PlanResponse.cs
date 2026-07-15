using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Plans;

public class PlanResponse
{
    public Guid Id { get; set; }
    public Guid GoalId { get; set; }
    public PlanType Type { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int RepeatCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
