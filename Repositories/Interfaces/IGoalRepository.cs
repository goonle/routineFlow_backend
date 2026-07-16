using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<List<Goal>> GetActiveAsync(Guid userId);
    Task<List<Goal>> GetDeletedAsync(Guid userId);
    Task<Goal?> GetByIdAsync(Guid userId, Guid goalId);
    Task<List<Goal>> GetSoftDeletedForRetentionScanAsync();
    Task AddAsync(Goal goal);
    void Remove(Goal goal);
    Task SaveChangesAsync();
}
