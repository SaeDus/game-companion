using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class CharacterExporter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly string outputFile = "character-state.json";

    public static bool HasNewData(string pathToCharacter, string pathToStash)
    {
        string outputPath = Path.Combine(AppContext.BaseDirectory, "Output", outputFile);

        if (!File.Exists(outputPath))
        {
            return true;
        }

        string json = File.ReadAllText(outputPath);

        using JsonDocument doc = JsonDocument.Parse(json);

        DateTimeOffset? previousCharacterSave = null;
        DateTimeOffset? previousStashSave = null;

        if (
            doc.RootElement.TryGetProperty(
                "CharacterSaveModifiedAt",
                out JsonElement characterElement
            )
        )
        {
            previousCharacterSave = characterElement.GetDateTimeOffset();
        }

        if (doc.RootElement.TryGetProperty("StashSaveModifiedAt", out JsonElement stashElement))
        {
            previousStashSave = stashElement.GetDateTimeOffset();
        }

        DateTimeOffset currentCharacterSave = new(
            File.GetLastWriteTimeUtc(pathToCharacter),
            TimeSpan.Zero
        );

        DateTimeOffset currentStashSave = new(File.GetLastWriteTimeUtc(pathToStash), TimeSpan.Zero);

        bool characterChanged =
            !previousCharacterSave.HasValue || previousCharacterSave.Value != currentCharacterSave;

        bool stashChanged =
            !previousStashSave.HasValue || previousStashSave.Value != currentStashSave;

        if (!characterChanged)
        {
            Console.WriteLine("WARNING: Character save has not changed since the previous export.");
        }

        if (!stashChanged)
        {
            Console.WriteLine("WARNING: Stash save has not changed since the previous export.");
        }

        return characterChanged || stashChanged;
    }

    public static string GetJsonFromGameState(GameState gameState)
    {
        return JsonSerializer.Serialize(gameState, Options);
    }
}
