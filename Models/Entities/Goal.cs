namespace RoutineFlow.Models.Entities;

public class Goal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DateTime? OneMonthWarningSentAt { get; set; }
    public DateTime? OneWeekWarningSentAt { get; set; }

    public User User { get; set; } = null!;
    public Plan? Plan { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
