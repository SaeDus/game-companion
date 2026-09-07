using System.Text.Json.Serialization;

public class SkillState
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public int Level { get; set; }
}
