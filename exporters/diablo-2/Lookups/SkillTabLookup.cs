using System.Text.Json;

public static class SkillTabLookup
{
    private static readonly Dictionary<int, SkillTabMetadata> _skillTabDatabase;

    static SkillTabLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "skilltabs.json");

        string json = File.ReadAllText(path);

        _skillTabDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, SkillTabMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetClass(int skillTabId)
    {
        return _skillTabDatabase.TryGetValue(skillTabId, out SkillTabMetadata? skillTab)
            ? skillTab.Class
            : $"Unknown Skill Tab ({skillTabId})";
    }

    public static string GetTree(int skillTabId)
    {
        return _skillTabDatabase.TryGetValue(skillTabId, out SkillTabMetadata? skillTab)
            ? skillTab.Tree
            : $"Unknown Skill Tab ({skillTabId})";
    }

    public static SkillTabMetadata? Get(int skillTabId)
    {
        return _skillTabDatabase.TryGetValue(skillTabId, out SkillTabMetadata? skillTab)
            ? skillTab
            : null;
    }
}
