using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface ITaskItemRepository
{
    Task<List<TaskItem>> GetByGoalAsync(Guid goalId);
    Task<TaskItem?> GetByIdAsync(Guid goalId, Guid taskId);
    Task<TaskItem?> GetOwnedAsync(Guid userId, Guid taskId);
    Task AddAsync(TaskItem task);
    void Remove(TaskItem task);
    Task SaveChangesAsync();
}
