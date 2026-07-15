using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class TaskCompletionRepository : ITaskCompletionRepository
{
    private readonly ApplicationDbContext _db;

    public TaskCompletionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<TaskCompletion?> GetByTaskAndDateAsync(Guid taskId, DateOnly date) =>
        _db.TaskCompletions.FirstOrDefaultAsync(c => c.TaskId == taskId && c.Date == date);

    public Task<List<TaskCompletion>> GetHistoryAsync(Guid taskId, DateOnly from, DateOnly to) =>
        _db.TaskCompletions
            .Where(c => c.TaskId == taskId && c.Date >= from && c.Date <= to)
            .OrderBy(c => c.Date)
            .ToListAsync();

    public async Task<TaskCompletion> UpsertIncrementAsync(Guid taskId, DateOnly date, string? note)
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await _db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO ""TaskCompletions"" (""Id"", ""TaskId"", ""Date"", ""Count"", ""Note"", ""CreatedAt"")
VALUES ({id}, {taskId}, {date}, 1, {note}, {now})
ON CONFLICT (""TaskId"", ""Date"")
DO UPDATE SET ""Count"" = ""TaskCompletions"".""Count"" + 1,
              ""Note"" = COALESCE({note}, ""TaskCompletions"".""Note"")");

        return await GetByTaskAndDateAsync(taskId, date)
            ?? throw new InvalidOperationException("Upserted TaskCompletion row could not be re-read.");
    }

    public void Remove(TaskCompletion completion) => _db.TaskCompletions.Remove(completion);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
