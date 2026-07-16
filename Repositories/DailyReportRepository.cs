using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class DailyReportRepository : IDailyReportRepository
{
    private readonly ApplicationDbContext _db;

    public DailyReportRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<DailyReport?> GetByUserAndDateAsync(Guid userId, DateOnly date) =>
        _db.DailyReports.FirstOrDefaultAsync(d => d.UserId == userId && d.Date == date);

    public Task<List<DailyReport>> GetByUserAndRangeAsync(Guid userId, DateOnly from, DateOnly to) =>
        _db.DailyReports.Where(d => d.UserId == userId && d.Date >= from && d.Date <= to).ToListAsync();

    public async Task AddAsync(DailyReport report) => await _db.DailyReports.AddAsync(report);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
