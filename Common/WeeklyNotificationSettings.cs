namespace RoutineFlow.Common;

public class WeeklyNotificationSettings
{
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;
    public int Hour { get; set; } = 8;
    public int CheckIntervalMinutes { get; set; } = 60;
}
