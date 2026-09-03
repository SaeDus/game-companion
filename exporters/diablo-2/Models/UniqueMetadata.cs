using System.Text.Json.Serialization;

public class UniqueMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}