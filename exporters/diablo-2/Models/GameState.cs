public sealed class GameState
{
    public int CharacterVersion { get; set; } = 1;
    public DateTimeOffset GeneratedAt { get; set; }

    public DateTimeOffset? CharacterSaveModifiedAt { get; set; }
    public DateTimeOffset? StashSaveModifiedAt { get; set; }

    public CharacterState Character { get; set; } = new();
    public MercenaryState Mercenary { get; set; } = new();
    public StashState Stash { get; set; } = new();

    public QuestLogState QuestLog { get; set; } = new();
    public ControlledResourcesState ControlledResources { get; set; } = new();

    public ChronicleState Chronicle { get; set; } = new();
}
