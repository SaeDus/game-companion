using System.Text.Json.Serialization;

public class SetItemMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}