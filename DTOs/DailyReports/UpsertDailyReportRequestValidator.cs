using FluentValidation;

namespace RoutineFlow.DTOs.DailyReports;

public class UpsertDailyReportRequestValidator : AbstractValidator<UpsertDailyReportRequest>
{
    public UpsertDailyReportRequestValidator()
    {
        RuleFor(x => x.Emotion).IsInEnum().When(x => x.Emotion.HasValue);
        RuleFor(x => x.DiaryText).MaximumLength(5000);
    }
}
