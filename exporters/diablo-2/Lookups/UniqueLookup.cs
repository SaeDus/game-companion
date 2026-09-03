using System.Text.Json;

public static class UniqueLookup
{
    private static readonly Dictionary<int, UniqueMetadata> _uniqueDatabase;

    static UniqueLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "uniqueitems.json");

        string json = File.ReadAllText(path);

        _uniqueDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, UniqueMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetName(int uniqueId)
    {
        return _uniqueDatabase.TryGetValue(uniqueId, out UniqueMetadata? unique)
            ? unique.Name
            : $"Unknown Unique ({uniqueId})";
    }

    public static UniqueMetadata? Get(int uniqueId)
    {
        return _uniqueDatabase.TryGetValue(uniqueId, out UniqueMetadata? unique) ? unique : null;
    }
}
