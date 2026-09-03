public class QuestLogState {
    public DifficultyState Normal { get; set; } = new();
    public DifficultyState Nightmare { get; set; } = new();
    public DifficultyState Hell { get; set; } = new();
}