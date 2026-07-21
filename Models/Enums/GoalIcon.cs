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
    Mindfulness,
    // Appended after initial release — keep new values at the end so
    // existing rows' stored ints keep pointing at the same icon.
    Fitness,
    Nutrition,
    Hydration,
    Motivation,
    Writing,
    Technology,
    Music,
    Celebration,
    Teamwork,
    Travel,
    Nature,
    Energy
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
        [GoalIcon.Mindfulness] = "🧘",
        [GoalIcon.Fitness] = "🏃",
        [GoalIcon.Nutrition] = "🥗",
        [GoalIcon.Hydration] = "💧",
        [GoalIcon.Motivation] = "🤩",
        [GoalIcon.Writing] = "✏️",
        [GoalIcon.Technology] = "💻",
        [GoalIcon.Music] = "🎵",
        [GoalIcon.Celebration] = "🎉",
        [GoalIcon.Teamwork] = "🤝",
        [GoalIcon.Travel] = "✈️",
        [GoalIcon.Nature] = "🌱",
        [GoalIcon.Energy] = "⚡"
    };

    public static string Emoji(GoalIcon icon) => Emojis[icon];
}
