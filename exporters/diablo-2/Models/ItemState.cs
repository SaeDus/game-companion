using System.Text.Json.Serialization;
using D2SSharp.Enums;

public class ItemState
{
    public uint? ItemSeed { get; set; }

    public string? Slot { get; set; }

    public string? Name { get; set; }
    public string BaseName { get; set; } = "";
    public string BaseCode { get; set; } = "";

    public int? ItemLevel { get; set; }
    public string? Quality { get; set; }

    public int? Quantity { get; set; }
    public int? BaseDefense { get; set; }

    public List<string>? Stats { get; set; }
    public List<SocketState>? Sockets { get; set; }

    public int? PosX { get; set; }
    public int? PosY { get; set; }

    [JsonIgnore]
    public ItemFlags? Flags { get; set; }
}
