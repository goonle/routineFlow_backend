using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<List<Goal>> GetActiveAsync(Guid userId);
    Task<List<Goal>> GetDeletedAsync(Guid userId);
    Task<Goal?> GetByIdAsync(Guid userId, Guid goalId);
    Task AddAsync(Goal goal);
    Task SaveChangesAsync();
}
