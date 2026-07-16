using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Calendar;

public class CalendarDayResponse
{
    public DateOnly Date { get; set; }
    public EmotionType? Emotion { get; set; }
    public string? Emoji { get; set; }
    public List<CalendarGoalResponse> Goals { get; set; } = new();
}
