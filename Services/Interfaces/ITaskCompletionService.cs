using RoutineFlow.DTOs.TaskCompletions;

namespace RoutineFlow.Services.Interfaces;

public interface ITaskCompletionService
{
    Task<TaskCompletionResponse> MarkDoneAsync(Guid userId, Guid taskId, MarkCompletionRequest request);
    Task<List<TaskCompletionResponse>> GetHistoryAsync(Guid userId, Guid taskId, DateOnly? from, DateOnly? to);
    Task UndoAsync(Guid userId, Guid taskId, DateOnly date);
}
