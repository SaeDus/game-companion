using System.Text.Json.Serialization;

public class MiscMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}