using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _db;

    public TaskItemRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<TaskItem>> GetByGoalAsync(Guid goalId) =>
        _db.Tasks.Where(t => t.GoalId == goalId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

    public Task<TaskItem?> GetByIdAsync(Guid goalId, Guid taskId) =>
        _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.GoalId == goalId);

    public Task<TaskItem?> GetOwnedAsync(Guid userId, Guid taskId) =>
        _db.Tasks.Include(t => t.Goal)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Goal.UserId == userId);

    public async Task AddAsync(TaskItem task) => await _db.Tasks.AddAsync(task);

    public void Remove(TaskItem task) => _db.Tasks.Remove(task);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
