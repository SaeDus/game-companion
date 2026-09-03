using System.Text.Json.Serialization;

public class WeaponMetadata {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}