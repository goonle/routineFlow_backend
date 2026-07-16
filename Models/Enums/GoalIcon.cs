namespace RoutineFlow.Models.Enums;

public enum GoalIcon
{
    General,
    Health,
    Learning,
    Finance,
    Career,
    Creativity,
    Social,
    Mindfulness
}

public static class GoalIconMetadata
{
    private static readonly Dictionary<GoalIcon, string> Emojis = new()
    {
        [GoalIcon.General] = "🎯",
        [GoalIcon.Health] = "🏋️",
        [GoalIcon.Learning] = "📚",
        [GoalIcon.Finance] = "💰",
        [GoalIcon.Career] = "💼",
        [GoalIcon.Creativity] = "🎨",
        [GoalIcon.Social] = "👥",
        [GoalIcon.Mindfulness] = "🧘"
    };

    public static string Emoji(GoalIcon icon) => Emojis[icon];
}
