using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Goals;

public class GoalResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalIcon Icon { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public GoalColor Color { get; set; }
    public string ColorHex { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
