using System.Text.Json;

public static class HirelingLookup
{
    private static readonly Dictionary<int, HirelingMetadata> _hirelingDatabase;

    static HirelingLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "hirelings.json");

        string json = File.ReadAllText(path);

        _hirelingDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, HirelingMetadata>>>(json)?.Data
            ?? [];
    }

    public static HirelingMetadata? Get(int hirelingId)
    {
        return _hirelingDatabase.TryGetValue(hirelingId, out HirelingMetadata? hireling)
            ? hireling
            : null;
    }
}

public class HirelingResolvedState
{
    public int Level { get; set; }
    public HirelingRow Row { get; set; } = new();
}
