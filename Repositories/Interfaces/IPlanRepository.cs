using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IPlanRepository
{
    Task<Plan?> GetByGoalIdAsync(Guid goalId);
    Task AddAsync(Plan plan);
    void Remove(Plan plan);
    Task SaveChangesAsync();
}
