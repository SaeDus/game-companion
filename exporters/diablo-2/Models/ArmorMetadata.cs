using System.Text.Json.Serialization;

public class ArmorMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}