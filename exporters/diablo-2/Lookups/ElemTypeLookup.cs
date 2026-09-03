using System.Text.Json;

public static class ElemTypeLookup
{
    private static readonly Dictionary<int, ElemTypeMetadata> _elemTypeDatabase;

    static ElemTypeLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "elemtypes.json");

        string json = File.ReadAllText(path);

        _elemTypeDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, ElemTypeMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetName(int elemTypeId)
    {
        return _elemTypeDatabase.TryGetValue(elemTypeId, out ElemTypeMetadata? elemType)
            ? elemType.Name
            : $"Unknown Element Type ({elemTypeId})";
    }

    public static ElemTypeMetadata? Get(int elemTypeId)
    {
        return _elemTypeDatabase.TryGetValue(elemTypeId, out ElemTypeMetadata? elemType)
            ? elemType
            : null;
    }
}
