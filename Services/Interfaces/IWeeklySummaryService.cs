namespace RoutineFlow.Services.Interfaces;

public interface IWeeklySummaryService
{
    Task<int> GenerateForAllUsersAsync();
}
