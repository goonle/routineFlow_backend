using RoutineFlow.DTOs.Plans;

namespace RoutineFlow.Services.Interfaces;

public interface IPlanService
{
    Task<PlanResponse> GetAsync(Guid userId, Guid goalId);
    Task<PlanResponse> UpsertAsync(Guid userId, Guid goalId, UpsertPlanRequest request);
    Task DeleteAsync(Guid userId, Guid goalId);
}
