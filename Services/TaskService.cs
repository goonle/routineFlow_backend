using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.Tasks;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class TaskService : ITaskService
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IGoalRepository _goalRepository;

    public TaskService(ITaskItemRepository taskRepository, IGoalRepository goalRepository)
    {
        _taskRepository = taskRepository;
        _goalRepository = goalRepository;
    }

    public async Task<List<TaskResponse>> ListAsync(Guid userId, Guid goalId)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var tasks = await _taskRepository.GetByGoalAsync(goalId);
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> GetAsync(Guid userId, Guid goalId, Guid taskId)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var task = await GetOwnedTaskAsync(goalId, taskId);
        return ToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(Guid userId, Guid goalId, CreateTaskRequest request)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            GoalId = goalId,
            Name = request.Name,
            Description = request.Description,
            AchieveAction = request.AchieveAction,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid userId, Guid goalId, Guid taskId, UpdateTaskRequest request)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var task = await GetOwnedTaskAsync(goalId, taskId);

        task.Name = request.Name;
        task.Description = request.Description;
        task.AchieveAction = request.AchieveAction;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task DeleteAsync(Guid userId, Guid goalId, Guid taskId)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var task = await GetOwnedTaskAsync(goalId, taskId);

        _taskRepository.Remove(task);
        await _taskRepository.SaveChangesAsync();
    }

    private async Task EnsureActiveGoalOwnedAsync(Guid userId, Guid goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(userId, goalId);
        if (goal is null || goal.DeletedAt is not null)
        {
            throw new NotFoundException("Goal not found.");
        }
    }

    private async Task<TaskItem> GetOwnedTaskAsync(Guid goalId, Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(goalId, taskId);
        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        return task;
    }

    private static TaskResponse ToResponse(TaskItem task) => new()
    {
        Id = task.Id,
        GoalId = task.GoalId,
        Name = task.Name,
        Description = task.Description,
        AchieveAction = task.AchieveAction,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
