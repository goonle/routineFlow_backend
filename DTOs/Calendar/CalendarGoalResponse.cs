using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Calendar;

public class CalendarGoalResponse
{
    public Guid GoalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public GoalIcon Icon { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public GoalColor Color { get; set; }
    public string ColorHex { get; set; } = string.Empty;
    public bool Achieved { get; set; }
}
