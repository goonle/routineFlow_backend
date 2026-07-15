using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.TaskCompletions;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class TaskCompletionService : ITaskCompletionService
{
    private readonly ITaskCompletionRepository _completionRepository;
    private readonly ITaskItemRepository _taskRepository;

    public TaskCompletionService(ITaskCompletionRepository completionRepository, ITaskItemRepository taskRepository)
    {
        _completionRepository = completionRepository;
        _taskRepository = taskRepository;
    }

    public async Task<TaskCompletionResponse> MarkDoneAsync(Guid userId, Guid taskId, MarkCompletionRequest request)
    {
        await EnsureOwnedTaskAsync(userId, taskId, requireActiveGoal: true);

        var date = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var completion = await _completionRepository.UpsertIncrementAsync(taskId, date, request.Note);

        return ToResponse(completion);
    }

    public async Task<List<TaskCompletionResponse>> GetHistoryAsync(Guid userId, Guid taskId, DateOnly? from, DateOnly? to)
    {
        await EnsureOwnedTaskAsync(userId, taskId, requireActiveGoal: false);

        var history = await _completionRepository.GetHistoryAsync(
            taskId,
            from ?? DateOnly.MinValue,
            to ?? DateOnly.MaxValue);

        return history.Select(ToResponse).ToList();
    }

    public async Task UndoAsync(Guid userId, Guid taskId, DateOnly date)
    {
        await EnsureOwnedTaskAsync(userId, taskId, requireActiveGoal: true);

        var completion = await _completionRepository.GetByTaskAndDateAsync(taskId, date);
        if (completion is null)
        {
            throw new NotFoundException("No completion recorded for this date.");
        }

        if (completion.Count <= 1)
        {
            _completionRepository.Remove(completion);
        }
        else
        {
            completion.Count -= 1;
        }

        await _completionRepository.SaveChangesAsync();
    }

    private async Task EnsureOwnedTaskAsync(Guid userId, Guid taskId, bool requireActiveGoal)
    {
        var task = await _taskRepository.GetOwnedAsync(userId, taskId);
        if (task is null || (requireActiveGoal && task.Goal.DeletedAt is not null))
        {
            throw new NotFoundException("Task not found.");
        }
    }

    private static TaskCompletionResponse ToResponse(TaskCompletion completion) => new()
    {
        Id = completion.Id,
        TaskId = completion.TaskId,
        Date = completion.Date,
        Count = completion.Count,
        Note = completion.Note,
        CreatedAt = completion.CreatedAt
    };
}
