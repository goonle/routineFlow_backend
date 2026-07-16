using System.Text.Json;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class WeeklySummaryService : IWeeklySummaryService
{
    private readonly IUserRepository _userRepository;
    private readonly IGoalRepository _goalRepository;
    private readonly ICalendarRepository _calendarRepository;
    private readonly INotificationRepository _notificationRepository;

    public WeeklySummaryService(
        IUserRepository userRepository,
        IGoalRepository goalRepository,
        ICalendarRepository calendarRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _goalRepository = goalRepository;
        _calendarRepository = calendarRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<int> GenerateForAllUsersAsync()
    {
        var now = DateTime.UtcNow;
        var currentWeekStart = StartOfWeek(DateOnly.FromDateTime(now));
        var summaryWeekStart = currentWeekStart.AddDays(-7);
        var summaryWeekEnd = currentWeekStart.AddDays(-1);

        var userIds = await _userRepository.GetAllIdsAsync();
        var currentWeekStartUtc = DateTime.SpecifyKind(currentWeekStart.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var created = 0;

        foreach (var userId in userIds)
        {
            var alreadyExists = await _notificationRepository.ExistsSinceAsync(
                userId, NotificationType.WeeklySummary, currentWeekStartUtc);
            if (alreadyExists)
            {
                continue;
            }

            var goals = await _goalRepository.GetActiveAsync(userId);
            var achievements = await _calendarRepository.GetAchievedGoalDatesAsync(userId, summaryWeekStart, summaryWeekEnd);
            var totalCompletions = await _calendarRepository.GetTotalCompletionCountAsync(userId, summaryWeekStart, summaryWeekEnd);
            var goalsAchieved = achievements.Select(a => a.GoalId).Distinct().Count();

            var content = JsonSerializer.Serialize(new
            {
                weekStart = summaryWeekStart,
                weekEnd = summaryWeekEnd,
                totalGoals = goals.Count,
                goalsAchievedAtLeastOnce = goalsAchieved,
                totalTaskCompletions = totalCompletions
            });

            await _notificationRepository.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.WeeklySummary,
                Content = content,
                IsRead = false,
                CreatedAt = now
            });
            created++;
        }

        if (created > 0)
        {
            await _notificationRepository.SaveChangesAsync();
        }

        return created;
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-daysSinceMonday);
    }
}
