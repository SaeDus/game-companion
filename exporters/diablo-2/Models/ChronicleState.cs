public class ChronicleState {
    public List<ChronicleMetadata> UniqueEntries { get; set; } = [];
    public List<ChronicleMetadata> SetEntries { get; set; } = [];
    public List<ChronicleMetadata>? RunewordEntries { get; set; }
}

public class ChronicleMetadata {
    public string Name { get; set; } = "";
    public string? Source { get; set; }
    public string? Time { get; set; }
}