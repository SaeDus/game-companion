using System.Text.Json.Serialization;

public class RuneMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("runes")]
    public List<string> Runes { get; set; } = [];
}