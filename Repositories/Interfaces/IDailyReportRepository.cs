using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IDailyReportRepository
{
    Task<DailyReport?> GetByUserAndDateAsync(Guid userId, DateOnly date);
    Task AddAsync(DailyReport report);
    Task SaveChangesAsync();
}
