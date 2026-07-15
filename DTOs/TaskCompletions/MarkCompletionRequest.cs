using System.ComponentModel.DataAnnotations;

namespace RoutineFlow.DTOs.TaskCompletions;

public class MarkCompletionRequest
{
    public DateOnly? Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}
