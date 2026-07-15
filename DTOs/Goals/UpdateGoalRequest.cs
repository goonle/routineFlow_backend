using System.ComponentModel.DataAnnotations;

namespace RoutineFlow.DTOs.Goals;

public class UpdateGoalRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
