using System.Text.Json;

public static class PlayerClassLookup
{
    private static readonly Dictionary<int, PlayerClassMetadata> _playerClassDatabase;

    static PlayerClassLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "playerclass.json");

        string json = File.ReadAllText(path);

        _playerClassDatabase =
            JsonSerializer
                .Deserialize<GeneratedJson<Dictionary<int, PlayerClassMetadata>>>(json)
                ?.Data
            ?? [];
    }

    public static string GetName(int classId)
    {
        return _playerClassDatabase.TryGetValue(classId, out PlayerClassMetadata? playerClass)
            ? playerClass.Name
            : $"Unknown Class ({classId})";
    }

    public static PlayerClassMetadata? Get(int classId)
    {
        return _playerClassDatabase.TryGetValue(classId, out PlayerClassMetadata? playerClass)
            ? playerClass
            : null;
    }
}
