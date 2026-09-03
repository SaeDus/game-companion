public class MercenaryState {
    public int Id { get; set; }
    public int NameIndex { get; set; }

    public string? Name { get; set; }
    public string? Hireling { get; set; }
    public string? SubType { get; set; }

    public int? Level { get; set; }
    public uint Experience { get; set; }

    public int? Strength { get; set; }
    public int? Dexterity { get; set; }

    public List<string>? Skills { get; set; }

    public List<ItemState> Equipment { get; set; } = [];
}