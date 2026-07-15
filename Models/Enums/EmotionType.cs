namespace RoutineFlow.Models.Enums;

public enum EmotionType
{
    Happy,
    Sad,
    Excited,
    Angry,
    Anxious,
    Calm,
    Tired,
    Neutral
}

public static class EmotionMetadata
{
    private static readonly Dictionary<EmotionType, string> Emojis = new()
    {
        [EmotionType.Happy] = "😊",
        [EmotionType.Sad] = "😢",
        [EmotionType.Excited] = "🤩",
        [EmotionType.Angry] = "😠",
        [EmotionType.Anxious] = "😰",
        [EmotionType.Calm] = "😌",
        [EmotionType.Tired] = "😴",
        [EmotionType.Neutral] = "😐"
    };

    public static string Emoji(EmotionType emotion) => Emojis[emotion];
}
