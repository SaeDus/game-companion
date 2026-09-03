using System.Text.Json.Serialization;

public class SkillMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("class")]
    public string Class { get; set; } = "";
}