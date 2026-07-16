using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.DailyReports;

public class UpsertDailyReportRequest
{
    public EmotionType? Emotion { get; set; }
    public string? DiaryText { get; set; }
}
