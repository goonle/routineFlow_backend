using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly ApplicationDbContext _db;

    public CalendarRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<GoalAchievement>> GetAchievedGoalDatesAsync(Guid userId, DateOnly from, DateOnly to) =>
        _db.TaskCompletions
            .Where(c => c.Count > 0 && c.Date >= from && c.Date <= to && c.Task.Goal.UserId == userId)
            .Select(c => new GoalAchievement(c.Task.GoalId, c.Date))
            .Distinct()
            .ToListAsync();

    public Task<int> GetTotalCompletionCountAsync(Guid userId, DateOnly from, DateOnly to) =>
        _db.TaskCompletions
            .Where(c => c.Date >= from && c.Date <= to && c.Task.Goal.UserId == userId)
            .SumAsync(c => c.Count);
}
