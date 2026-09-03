using System.Text.Json.Serialization;

public class PlayerClassMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}