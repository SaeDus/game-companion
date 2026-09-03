using System.Text.Json;

public static class SetItemLookup
{
    private static readonly Dictionary<int, SetItemMetadata> _setItemDatabase;

    static SetItemLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "setitems.json");

        string json = File.ReadAllText(path);

        _setItemDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, SetItemMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetName(int setItemId)
    {
        return _setItemDatabase.TryGetValue(setItemId, out SetItemMetadata? setItem)
            ? setItem.Name
            : $"Unknown Set Item ({setItemId})";
    }

    public static SetItemMetadata? Get(int setItemId)
    {
        return _setItemDatabase.TryGetValue(setItemId, out SetItemMetadata? setItem)
            ? setItem
            : null;
    }
}
