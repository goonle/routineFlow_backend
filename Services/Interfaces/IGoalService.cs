using RoutineFlow.DTOs.Goals;

namespace RoutineFlow.Services.Interfaces;

public interface IGoalService
{
    Task<List<GoalResponse>> ListAsync(Guid userId);
    Task<List<GoalResponse>> ListDeletedAsync(Guid userId);
    Task<GoalResponse> GetAsync(Guid userId, Guid goalId);
    Task<GoalResponse> CreateAsync(Guid userId, CreateGoalRequest request);
    Task<GoalResponse> UpdateAsync(Guid userId, Guid goalId, UpdateGoalRequest request);
    Task DeleteAsync(Guid userId, Guid goalId);
    Task<GoalResponse> RestoreAsync(Guid userId, Guid goalId);
}
