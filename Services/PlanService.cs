using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.Plans;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IGoalRepository _goalRepository;

    public PlanService(IPlanRepository planRepository, IGoalRepository goalRepository)
    {
        _planRepository = planRepository;
        _goalRepository = goalRepository;
    }

    public async Task<PlanResponse> GetAsync(Guid userId, Guid goalId)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var plan = await _planRepository.GetByGoalIdAsync(goalId);
        if (plan is null)
        {
            throw new NotFoundException("Plan not found.");
        }

        return ToResponse(plan);
    }

    public async Task<PlanResponse> UpsertAsync(Guid userId, Guid goalId, UpsertPlanRequest request)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var repeatCount = request.RepeatCount ?? 1;
        var now = DateTime.UtcNow;

        var plan = await _planRepository.GetByGoalIdAsync(goalId);
        if (plan is null)
        {
            plan = new Plan
            {
                Id = Guid.NewGuid(),
                GoalId = goalId,
                Type = request.Type,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RepeatCount = repeatCount,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _planRepository.AddAsync(plan);
        }
        else
        {
            plan.Type = request.Type;
            plan.StartDate = request.StartDate;
            plan.EndDate = request.EndDate;
            plan.RepeatCount = repeatCount;
            plan.UpdatedAt = now;
        }

        await _planRepository.SaveChangesAsync();

        return ToResponse(plan);
    }

    public async Task DeleteAsync(Guid userId, Guid goalId)
    {
        await EnsureActiveGoalOwnedAsync(userId, goalId);

        var plan = await _planRepository.GetByGoalIdAsync(goalId);
        if (plan is null)
        {
            throw new NotFoundException("Plan not found.");
        }

        _planRepository.Remove(plan);
        await _planRepository.SaveChangesAsync();
    }

    private async Task EnsureActiveGoalOwnedAsync(Guid userId, Guid goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(userId, goalId);
        if (goal is null || goal.DeletedAt is not null)
        {
            throw new NotFoundException("Goal not found.");
        }
    }

    private static PlanResponse ToResponse(Plan plan) => new()
    {
        Id = plan.Id,
        GoalId = plan.GoalId,
        Type = plan.Type,
        StartDate = plan.StartDate,
        EndDate = plan.EndDate,
        RepeatCount = plan.RepeatCount,
        CreatedAt = plan.CreatedAt,
        UpdatedAt = plan.UpdatedAt
    };
}
