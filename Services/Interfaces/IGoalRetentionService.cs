namespace RoutineFlow.Services.Interfaces;

public record GoalRetentionResult(int OneMonthWarnings, int OneWeekWarnings, int Purged);

public interface IGoalRetentionService
{
    Task<GoalRetentionResult> RunAsync();
}
