using System.Text.Json.Serialization;

public class SetMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}