using FluentValidation;

namespace RoutineFlow.DTOs.Tasks;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AchieveAction).NotEmpty().MaximumLength(500);
    }
}
