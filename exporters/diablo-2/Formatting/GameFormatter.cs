using D2SSharp.Model;

public class GameFormatter
{
    public static GameState BuildGameState(
        D2Save save,
        D2StashSave stash,
        string pathToCharacter,
        string pathToStash
    )
    {
        GameState gameState = new()
        {
            CharacterVersion = 1,
            GeneratedAt = DateTimeOffset.UtcNow,

            CharacterSaveModifiedAt = new DateTimeOffset(
                File.GetLastWriteTimeUtc(pathToCharacter),
                TimeSpan.Zero
            ),

            StashSaveModifiedAt = new DateTimeOffset(
                File.GetLastWriteTimeUtc(pathToStash),
                TimeSpan.Zero
            ),

            Character = CharacterFormatter.BuildCharacterState(save),
            Mercenary = CharacterFormatter.BuildMercenaryState(save),
            Stash = CharacterFormatter.BuildStashState(stash),
            QuestLog = QuestLogFormatter.BuildQuestLogState(save),

            ControlledResources = new()
            {
                AkaraRespecCount = 2,
                CharsiImbueCount = 2,
                LarzukSocketCount = 2,
            },

            Chronicle = ChronicleFormatter.BuildChronicleState(stash),
        };

        return gameState;
    }
}
