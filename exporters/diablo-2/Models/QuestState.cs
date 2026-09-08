using System.Text.Json.Serialization;

public class QuestState
{
    public string Name { get; set; } = "";
    public bool IsOptional { get; set; } = false;
    public string Status { get; set; } = "";

    [JsonIgnore]
    public string? Flags { get; set; }
}
