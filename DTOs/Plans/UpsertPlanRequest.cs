using System.ComponentModel.DataAnnotations;
using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Plans;

public class UpsertPlanRequest
{
    [Required]
    public PlanType Type { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? RepeatCount { get; set; }
}
