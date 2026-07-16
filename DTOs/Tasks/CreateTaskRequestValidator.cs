using FluentValidation;

namespace RoutineFlow.DTOs.Tasks;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AchieveAction).NotEmpty().MaximumLength(500);
    }
}
