using System.Text.Json;
using D2SSharp.Model;

public static class RuneLookup
{
    private static readonly Dictionary<string, RuneMetadata> _runeDatabase;

    static RuneLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "runes.json");

        string json = File.ReadAllText(path);

        _runeDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<string, RuneMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetName(Item item)
    {
        if (item.Sockets == null || item.Sockets.Count == 0)
        {
            return "";
        }

        List<string> socketCodes =
        [
            .. item
                .Sockets.Where(socket => socket != null)
                .Select(socket => socket!.ItemCodeString),
        ];

        foreach (RuneMetadata rune in _runeDatabase.Values)
        {
            if (rune.Runes.Count != socketCodes.Count)
            {
                continue;
            }

            if (rune.Runes.SequenceEqual(socketCodes))
            {
                return rune.Name;
            }
        }

        return $"Unknown Runeword ({item.RunewordId})";
    }

    public static RuneMetadata? Get(string runeId)
    {
        return _runeDatabase.TryGetValue(runeId, out RuneMetadata? rune) ? rune : null;
    }
}
