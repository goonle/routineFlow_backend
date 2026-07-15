using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface ITaskCompletionRepository
{
    Task<TaskCompletion?> GetByTaskAndDateAsync(Guid taskId, DateOnly date);
    Task<List<TaskCompletion>> GetHistoryAsync(Guid taskId, DateOnly from, DateOnly to);
    Task<TaskCompletion> UpsertIncrementAsync(Guid taskId, DateOnly date, string? note);
    void Remove(TaskCompletion completion);
    Task SaveChangesAsync();
}
