using System.Text.Json;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class GoalRetentionService : IGoalRetentionService
{
    private const int RetentionMonths = 6;
    private const int WarningMonthsBeforePurge = 1;
    private const int WarningDaysBeforePurge = 7;

    private readonly IGoalRepository _goalRepository;
    private readonly INotificationRepository _notificationRepository;

    public GoalRetentionService(IGoalRepository goalRepository, INotificationRepository notificationRepository)
    {
        _goalRepository = goalRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<GoalRetentionResult> RunAsync()
    {
        var now = DateTime.UtcNow;
        var goals = await _goalRepository.GetSoftDeletedForRetentionScanAsync();

        var oneMonthWarnings = 0;
        var oneWeekWarnings = 0;
        var purged = 0;

        foreach (var goal in goals)
        {
            var deletedAt = goal.DeletedAt!.Value;
            var purgeAt = deletedAt.AddMonths(RetentionMonths);

            if (now >= purgeAt)
            {
                _goalRepository.Remove(goal);
                purged++;
                continue;
            }

            var oneMonthWarningAt = purgeAt.AddMonths(-WarningMonthsBeforePurge);
            if (now >= oneMonthWarningAt && goal.OneMonthWarningSentAt is null)
            {
                await _notificationRepository.AddAsync(BuildWarningNotification(goal, "OneMonth", purgeAt, now));
                goal.OneMonthWarningSentAt = now;
                oneMonthWarnings++;
            }

            var oneWeekWarningAt = purgeAt.AddDays(-WarningDaysBeforePurge);
            if (now >= oneWeekWarningAt && goal.OneWeekWarningSentAt is null)
            {
                await _notificationRepository.AddAsync(BuildWarningNotification(goal, "OneWeek", purgeAt, now));
                goal.OneWeekWarningSentAt = now;
                oneWeekWarnings++;
            }
        }

        if (oneMonthWarnings + oneWeekWarnings + purged > 0)
        {
            await _goalRepository.SaveChangesAsync();
        }

        return new GoalRetentionResult(oneMonthWarnings, oneWeekWarnings, purged);
    }

    private static Notification BuildWarningNotification(Goal goal, string stage, DateTime purgeAt, DateTime now) => new()
    {
        Id = Guid.NewGuid(),
        UserId = goal.UserId,
        Type = NotificationType.GoalPurgeWarning,
        Content = JsonSerializer.Serialize(new
        {
            goalId = goal.Id,
            goalName = goal.Name,
            stage,
            purgeAt
        }),
        IsRead = false,
        CreatedAt = now
    };
}
