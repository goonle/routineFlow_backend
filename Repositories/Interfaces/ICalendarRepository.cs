namespace RoutineFlow.Repositories.Interfaces;

public record GoalAchievement(Guid GoalId, DateOnly Date);

public interface ICalendarRepository
{
    Task<List<GoalAchievement>> GetAchievedGoalDatesAsync(Guid userId, DateOnly from, DateOnly to);
    Task<int> GetTotalCompletionCountAsync(Guid userId, DateOnly from, DateOnly to);
}
