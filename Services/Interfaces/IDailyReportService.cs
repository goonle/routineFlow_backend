using RoutineFlow.DTOs.DailyReports;

namespace RoutineFlow.Services.Interfaces;

public interface IDailyReportService
{
    Task<DailyReportResponse> GetAsync(Guid userId, DateOnly date);
    Task<DailyReportResponse> UpsertAsync(Guid userId, DateOnly date, UpsertDailyReportRequest request);
}
