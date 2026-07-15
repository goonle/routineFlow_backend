using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.DailyReports;

public class DailyReportResponse
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public EmotionType? Emotion { get; set; }
    public string? Emoji { get; set; }
    public string? DiaryText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
