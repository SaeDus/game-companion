using System.Text.Json.Serialization;

public class SkillTabMetadata {
    [JsonPropertyName("class")]
    public string Class { get; set; } = "";

    [JsonPropertyName("tree")]
    public string Tree { get; set; } = "";
}