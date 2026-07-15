using RoutineFlow.Models.Enums;

namespace RoutineFlow.Models.Entities;

public class DailyReport
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public EmotionType? Emotion { get; set; }
    public string? DiaryText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
