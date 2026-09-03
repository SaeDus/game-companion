using System.Text.Json;

public static class MiscLookup
{
    private static readonly Dictionary<string, MiscMetadata> _miscDatabase;

    static MiscLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "misc.json");

        string json = File.ReadAllText(path);

        _miscDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<string, MiscMetadata>>>(json)?.Data
            ?? [];
    }

    public static bool TryGetName(string itemId, out string miscName)
    {
        if (!_miscDatabase.TryGetValue(itemId, out MiscMetadata? misc))
        {
            miscName = "";
            return false;
        }

        miscName = misc.Name;
        return true;
    }

    public static MiscMetadata? Get(string itemId)
    {
        return _miscDatabase.TryGetValue(itemId, out MiscMetadata? misc) ? misc : null;
    }
}
