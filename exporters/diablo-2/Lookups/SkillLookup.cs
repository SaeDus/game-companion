using System.Text.Json;

public static class SkillLookup
{
    private static readonly Dictionary<int, SkillMetadata> _skillDatabase;

    static SkillLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "skills.json");

        string json = File.ReadAllText(path);

        _skillDatabase =
            JsonSerializer.Deserialize<GeneratedJson<Dictionary<int, SkillMetadata>>>(json)?.Data
            ?? [];
    }

    public static string GetName(int skillId)
    {
        return _skillDatabase.TryGetValue(skillId, out SkillMetadata? skill)
            ? skill.Name
            : $"Unknown Skill ({skillId})";
    }

    public static SkillMetadata? Get(int skillId)
    {
        return _skillDatabase.TryGetValue(skillId, out SkillMetadata? skill) ? skill : null;
    }
}
