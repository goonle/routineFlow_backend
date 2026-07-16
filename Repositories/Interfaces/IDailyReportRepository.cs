using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IDailyReportRepository
{
    Task<DailyReport?> GetByUserAndDateAsync(Guid userId, DateOnly date);
    Task<List<DailyReport>> GetByUserAndRangeAsync(Guid userId, DateOnly from, DateOnly to);
    Task AddAsync(DailyReport report);
    Task SaveChangesAsync();
}
