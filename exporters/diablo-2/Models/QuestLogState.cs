public class QuestLogState
{
    public string? QuestLogStatus { get; set; }

    public DifficultyState? Normal { get; set; } = new();
    public string? NormalStatus { get; set; }

    public DifficultyState? Nightmare { get; set; } = new();
    public string? NightmareStatus { get; set; }

    public DifficultyState? Hell { get; set; } = new();
    public string? HellStatus { get; set; }
}
