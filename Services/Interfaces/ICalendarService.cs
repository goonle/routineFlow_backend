using RoutineFlow.DTOs.Calendar;

namespace RoutineFlow.Services.Interfaces;

public interface ICalendarService
{
    Task<List<CalendarDayResponse>> GetWeeklyAsync(Guid userId, DateOnly? startDate);
    Task<List<CalendarDayResponse>> GetMonthlyAsync(Guid userId, int year, int month);
}
