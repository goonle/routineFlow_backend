using RoutineFlow.DTOs.Calendar;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class CalendarService : ICalendarService
{
    private readonly IGoalRepository _goalRepository;
    private readonly IDailyReportRepository _dailyReportRepository;
    private readonly ICalendarRepository _calendarRepository;

    public CalendarService(
        IGoalRepository goalRepository,
        IDailyReportRepository dailyReportRepository,
        ICalendarRepository calendarRepository)
    {
        _goalRepository = goalRepository;
        _dailyReportRepository = dailyReportRepository;
        _calendarRepository = calendarRepository;
    }

    public Task<List<CalendarDayResponse>> GetWeeklyAsync(Guid userId, DateOnly? startDate)
    {
        var start = startDate ?? StartOfCurrentWeek();
        return BuildRangeAsync(userId, start, start.AddDays(6));
    }

    public Task<List<CalendarDayResponse>> GetMonthlyAsync(Guid userId, int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12.");
        }

        var start = new DateOnly(year, month, 1);
        return BuildRangeAsync(userId, start, start.AddMonths(1).AddDays(-1));
    }

    private static DateOnly StartOfCurrentWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        return today.AddDays(-daysSinceMonday);
    }

    private async Task<List<CalendarDayResponse>> BuildRangeAsync(Guid userId, DateOnly start, DateOnly end)
    {
        var goals = await _goalRepository.GetActiveAsync(userId);
        var reports = await _dailyReportRepository.GetByUserAndRangeAsync(userId, start, end);
        var achievements = await _calendarRepository.GetAchievedGoalDatesAsync(userId, start, end);

        var reportsByDate = reports.ToDictionary(r => r.Date);
        var achievedSet = achievements.Select(a => (a.GoalId, a.Date)).ToHashSet();

        var days = new List<CalendarDayResponse>();
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            reportsByDate.TryGetValue(date, out var report);
            days.Add(new CalendarDayResponse
            {
                Date = date,
                Emotion = report?.Emotion,
                Emoji = report?.Emotion is EmotionType emotion ? EmotionMetadata.Emoji(emotion) : null,
                Goals = goals.Select(g => new CalendarGoalResponse
                {
                    GoalId = g.Id,
                    Name = g.Name,
                    Icon = g.Icon,
                    Emoji = GoalIconMetadata.Emoji(g.Icon),
                    Achieved = achievedSet.Contains((g.Id, date))
                }).ToList()
            });
        }

        return days;
    }
}
