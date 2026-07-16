using FluentValidation;
using RoutineFlow.Models.Enums;

namespace RoutineFlow.DTOs.Plans;

public class UpsertPlanRequestValidator : AbstractValidator<UpsertPlanRequest>
{
    public UpsertPlanRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.RepeatCount)
            .GreaterThanOrEqualTo(1)
            .When(x => x.RepeatCount.HasValue)
            .WithMessage("RepeatCount must be at least 1.");

        When(x => x.Type == PlanType.Custom, () =>
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Custom plans require both StartDate and EndDate.");
            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage("Custom plans require both StartDate and EndDate.");
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate is not null && x.EndDate is not null)
                .WithMessage("EndDate must be on or after StartDate.");
        }).Otherwise(() =>
        {
            RuleFor(x => x.StartDate)
                .Null()
                .WithMessage("StartDate/EndDate are only valid for Custom plans.");
            RuleFor(x => x.EndDate)
                .Null()
                .WithMessage("StartDate/EndDate are only valid for Custom plans.");
        });
    }
}
