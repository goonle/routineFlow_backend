using System.ComponentModel.DataAnnotations;
using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.DailyReports;

public class UpsertDailyReportRequest
{
    public EmotionType? Emotion { get; set; }

    [MaxLength(5000)]
    public string? DiaryText { get; set; }
}
