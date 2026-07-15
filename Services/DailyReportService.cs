using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.DailyReports;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class DailyReportService : IDailyReportService
{
    private readonly IDailyReportRepository _dailyReportRepository;

    public DailyReportService(IDailyReportRepository dailyReportRepository)
    {
        _dailyReportRepository = dailyReportRepository;
    }

    public async Task<DailyReportResponse> GetAsync(Guid userId, DateOnly date)
    {
        var report = await _dailyReportRepository.GetByUserAndDateAsync(userId, date);
        if (report is null)
        {
            throw new NotFoundException("Daily report not found.");
        }

        return ToResponse(report);
    }

    public async Task<DailyReportResponse> UpsertAsync(Guid userId, DateOnly date, UpsertDailyReportRequest request)
    {
        var now = DateTime.UtcNow;
        var report = await _dailyReportRepository.GetByUserAndDateAsync(userId, date);

        if (report is null)
        {
            report = new DailyReport
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                Emotion = request.Emotion,
                DiaryText = request.DiaryText,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _dailyReportRepository.AddAsync(report);
        }
        else
        {
            report.Emotion = request.Emotion;
            report.DiaryText = request.DiaryText;
            report.UpdatedAt = now;
        }

        await _dailyReportRepository.SaveChangesAsync();

        return ToResponse(report);
    }

    private static DailyReportResponse ToResponse(DailyReport report) => new()
    {
        Id = report.Id,
        Date = report.Date,
        Emotion = report.Emotion,
        Emoji = report.Emotion is EmotionType emotion ? EmotionMetadata.Emoji(emotion) : null,
        DiaryText = report.DiaryText,
        CreatedAt = report.CreatedAt,
        UpdatedAt = report.UpdatedAt
    };
}
