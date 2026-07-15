using RoutineFlow.DTOs.Tasks;

namespace RoutineFlow.Services.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponse>> ListAsync(Guid userId, Guid goalId);
    Task<TaskResponse> GetAsync(Guid userId, Guid goalId, Guid taskId);
    Task<TaskResponse> CreateAsync(Guid userId, Guid goalId, CreateTaskRequest request);
    Task<TaskResponse> UpdateAsync(Guid userId, Guid goalId, Guid taskId, UpdateTaskRequest request);
    Task DeleteAsync(Guid userId, Guid goalId, Guid taskId);
}
