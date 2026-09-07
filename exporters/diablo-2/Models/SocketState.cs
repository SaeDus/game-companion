using System.Text.Json.Serialization;

public class SocketState
{
    public bool IsEmpty { get; set; }

    [JsonIgnore]
    public string? BaseCode { get; set; }
    public string? BaseName { get; set; }
}
