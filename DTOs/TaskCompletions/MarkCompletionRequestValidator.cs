using FluentValidation;

namespace RoutineFlow.DTOs.TaskCompletions;

public class MarkCompletionRequestValidator : AbstractValidator<MarkCompletionRequest>
{
    public MarkCompletionRequestValidator()
    {
        RuleFor(x => x.Note).MaximumLength(500);
    }
}
