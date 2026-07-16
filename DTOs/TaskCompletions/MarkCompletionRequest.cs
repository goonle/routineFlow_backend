namespace RoutineFlow.DTOs.TaskCompletions;

public class MarkCompletionRequest
{
    public DateOnly? Date { get; set; }
    public string? Note { get; set; }
}
