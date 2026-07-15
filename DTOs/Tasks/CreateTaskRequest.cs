using System.ComponentModel.DataAnnotations;

namespace RoutineFlow.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required, MaxLength(500)]
    public string AchieveAction { get; set; } = string.Empty;
}
