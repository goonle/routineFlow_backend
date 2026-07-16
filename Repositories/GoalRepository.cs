using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly ApplicationDbContext _db;

    public GoalRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<Goal>> GetActiveAsync(Guid userId) =>
        _db.Goals.Where(g => g.UserId == userId && g.DeletedAt == null)
            .OrderBy(g => g.CreatedAt)
            .ToListAsync();

    public Task<List<Goal>> GetDeletedAsync(Guid userId) =>
        _db.Goals.Where(g => g.UserId == userId && g.DeletedAt != null)
            .OrderBy(g => g.DeletedAt)
            .ToListAsync();

    public Task<Goal?> GetByIdAsync(Guid userId, Guid goalId) =>
        _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

    public Task<List<Goal>> GetSoftDeletedForRetentionScanAsync() =>
        _db.Goals.Where(g => g.DeletedAt != null)
            .OrderBy(g => g.DeletedAt)
            .ToListAsync();

    public async Task AddAsync(Goal goal) => await _db.Goals.AddAsync(goal);

    public void Remove(Goal goal) => _db.Goals.Remove(goal);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
