namespace RoutineFlow.DTOs.Calendar;

public class CalendarGoalResponse
{
    public Guid GoalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Achieved { get; set; }
}
