using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.Goals;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;

    public GoalService(IGoalRepository goalRepository)
    {
        _goalRepository = goalRepository;
    }

    public async Task<List<GoalResponse>> ListAsync(Guid userId)
    {
        var goals = await _goalRepository.GetActiveAsync(userId);
        return goals.Select(ToResponse).ToList();
    }

    public async Task<List<GoalResponse>> ListDeletedAsync(Guid userId)
    {
        var goals = await _goalRepository.GetDeletedAsync(userId);
        return goals.Select(ToResponse).ToList();
    }

    public async Task<GoalResponse> GetAsync(Guid userId, Guid goalId)
    {
        var goal = await GetActiveOwnedGoalAsync(userId, goalId);
        return ToResponse(goal);
    }

    public async Task<GoalResponse> CreateAsync(Guid userId, CreateGoalRequest request)
    {
        var now = DateTime.UtcNow;
        var goal = new Goal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _goalRepository.AddAsync(goal);
        await _goalRepository.SaveChangesAsync();

        return ToResponse(goal);
    }

    public async Task<GoalResponse> UpdateAsync(Guid userId, Guid goalId, UpdateGoalRequest request)
    {
        var goal = await GetActiveOwnedGoalAsync(userId, goalId);

        goal.Name = request.Name;
        goal.Description = request.Description;
        goal.UpdatedAt = DateTime.UtcNow;

        await _goalRepository.SaveChangesAsync();

        return ToResponse(goal);
    }

    public async Task DeleteAsync(Guid userId, Guid goalId)
    {
        var goal = await GetActiveOwnedGoalAsync(userId, goalId);

        goal.DeletedAt = DateTime.UtcNow;

        await _goalRepository.SaveChangesAsync();
    }

    public async Task<GoalResponse> RestoreAsync(Guid userId, Guid goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(userId, goalId);
        if (goal is null || goal.DeletedAt is null)
        {
            throw new NotFoundException("Deleted goal not found.");
        }

        goal.DeletedAt = null;
        goal.OneMonthWarningSentAt = null;
        goal.OneWeekWarningSentAt = null;
        goal.UpdatedAt = DateTime.UtcNow;

        await _goalRepository.SaveChangesAsync();

        return ToResponse(goal);
    }

    private async Task<Goal> GetActiveOwnedGoalAsync(Guid userId, Guid goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(userId, goalId);
        if (goal is null || goal.DeletedAt is not null)
        {
            throw new NotFoundException("Goal not found.");
        }

        return goal;
    }

    private static GoalResponse ToResponse(Goal goal) => new()
    {
        Id = goal.Id,
        Name = goal.Name,
        Description = goal.Description,
        CreatedAt = goal.CreatedAt,
        UpdatedAt = goal.UpdatedAt,
        DeletedAt = goal.DeletedAt
    };
}
