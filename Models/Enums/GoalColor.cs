namespace RoutineFlow.Models.Enums;

public enum GoalColor
{
    Red,
    Orange,
    Yellow,
    Green,
    Sky,
    Blue,
    Purple,
    Pink,
    Brown,
    Teal
}

public static class GoalColorMetadata
{
    private static readonly Dictionary<GoalColor, string> HexValues = new()
    {
        [GoalColor.Red] = "#ef4444",
        [GoalColor.Orange] = "#f97316",
        [GoalColor.Yellow] = "#eab308",
        [GoalColor.Green] = "#22c55e",
        [GoalColor.Sky] = "#0ea5e9",
        [GoalColor.Blue] = "#3b82f6",
        [GoalColor.Purple] = "#a855f7",
        [GoalColor.Pink] = "#ec4899",
        [GoalColor.Brown] = "#92400e",
        [GoalColor.Teal] = "#14b8a6"
    };

    public static string Hex(GoalColor color) => HexValues[color];
}
