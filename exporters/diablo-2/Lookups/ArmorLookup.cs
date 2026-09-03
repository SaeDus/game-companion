using System.Text.Json;

public static class ArmorLookup
{
    private static readonly Dictionary<string, ArmorMetadata> _armorDatabase;

    static ArmorLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "armor.json");

        string json = File.ReadAllText(path);

        _armorDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<string, ArmorMetadata>>>(json)?.Data
            ?? [];
    }

    public static bool TryGetName(string itemId, out string armorName)
    {
        if (!_armorDatabase.TryGetValue(itemId, out ArmorMetadata? armor))
        {
            armorName = "";
            return false;
        }

        armorName = armor.Name;
        return true;
    }

    public static ArmorMetadata? Get(string itemId)
    {
        return _armorDatabase.TryGetValue(itemId, out ArmorMetadata? armor) ? armor : null;
    }
}
