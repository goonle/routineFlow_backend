using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly ApplicationDbContext _db;

    public PlanRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Plan?> GetByGoalIdAsync(Guid goalId) =>
        _db.Plans.FirstOrDefaultAsync(p => p.GoalId == goalId);

    public async Task AddAsync(Plan plan) => await _db.Plans.AddAsync(plan);

    public void Remove(Plan plan) => _db.Plans.Remove(plan);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
