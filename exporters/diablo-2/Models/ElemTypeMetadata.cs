using System.Text.Json.Serialization;

public class ElemTypeMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
