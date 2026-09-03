using System.Text.Json.Serialization;

public class GeneratedJson<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}
