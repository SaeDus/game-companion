using D2SSharp.Enums;
using D2SSharp.Model;

public static class ItemFormatter
{
    public static bool TryBuildItem(Item item, bool getAllItems, out ItemState itemState)
    {
        itemState = new();

        // skips runes, etc. that have a quantity of 0 in the stash
        if (item.AdvancedStashStackSize != null && item.AdvancedStashStackSize == 0)
        {
            return false;
        }

        if (item.ItemSeed != 0)
        {
            itemState.ItemSeed = item.ItemSeed;
        }

        if (TryGetItemName(item, out var itemName))
        {
            itemState.Name = itemName;
        }

        if (!TryGetBaseItemType(item.ItemCodeString, out var itemType))
        {
            if (!getAllItems)
            {
                itemState = new ItemState();
                return false;
            }

            itemType = $"[{itemType}]";
        }

        itemState.BaseName = itemType;
        itemState.BaseCode = item.ItemCodeString;

        if (item.Position.BodyLocation != BodyLocation.None)
        {
            itemState.Slot = item.Position.BodyLocation.ToString();
        }
        else
        {
            if (item.AdvancedStashStackSize == null)
            {
                itemState.PosX = item.Position.InvX;
                itemState.PosY = item.Position.InvY;
            }
        }

        if (item.AdvancedStashStackSize == null)
        {
            itemState.ItemLevel = item.ItemLevel;
            itemState.Quality = item.Quality.ToString();
        }
        else
        {
            itemState.Quantity = item.AdvancedStashStackSize;
            return true;
        }

        if (item.Defense != null)
        {
            itemState.BaseDefense = item.Defense;
        }

        if (item.RunewordId != null && item.RunewordStats != null)
        {
            itemState.Stats ??= [];

            foreach (var runeStatString in FormatAllStats(item.RunewordStats))
            {
                itemState.Stats.Add(runeStatString);
            }
        }

        if (item.Stats != null && item.Stats.Count > 0)
        {
            itemState.Stats ??= [];

            foreach (var statString in FormatAllStats(item.Stats))
            {
                itemState.Stats.Add(statString);
            }
        }

        if ((item.Flags & ItemFlags.Ethereal) != 0)
        {
            (itemState.Stats ??= []).Add("Ethereal (Cannot be Repaired)");
        }

        if (item.Sockets.Count > 0)
        {
            itemState.Sockets = [];

            foreach (var socket in item.Sockets)
            {
                itemState.Sockets.Add(BuildSocketState(socket));
            }
        }

        itemState.Flags = item.Flags;

        return true;
    }

    private static bool TryGetItemName(Item item, out string itemName)
    {
        if (item.RunewordId != null)
        {
            itemName = RuneLookup.GetName(item);
            return true;
        }
        else if (item.Quality == ItemQuality.Unique)
        {
            if (item.QualityData != null && item.QualityData.FileIndex != null)
            {
                itemName = UniqueLookup.GetName((int)item.QualityData.FileIndex);
                return true;
            }
        }
        else if (item.Quality == ItemQuality.Set)
        {
            if (item.QualityData != null && item.QualityData.FileIndex != null)
            {
                itemName = SetItemLookup.GetName((int)item.QualityData.FileIndex);
                return true;
            }
        }

        // Handle magic/rare prefix/suffix names here

        itemName = "";
        return false;
    }

    private static bool TryGetBaseItemType(string id, out string itemName)
    {
        if (ArmorLookup.TryGetName(id, out itemName))
        {
            return true;
        }

        if (MiscLookup.TryGetName(id, out itemName))
        {
            return true;
        }

        if (WeaponLookup.TryGetName(id, out itemName))
        {
            return true;
        }

        // item is not important enough to list
        itemName = id;
        return false;
    }

    private static IEnumerable<string> FormatAllStats(IEnumerable<Stat> allStats)
    {
        foreach (var stat in allStats)
        {
            if (!StatFormatter.TryFormatStat(stat, allStats, out var statText))
            {
                continue;
            }

            yield return statText;
        }
    }

    private static SocketState BuildSocketState(Item? socket)
    {
        if (socket?.ItemCodeString is not string code)
        {
            return new SocketState { IsEmpty = true };
        }

        MiscLookup.TryGetName(code, out var name);

        return new SocketState
        {
            IsEmpty = false,
            BaseCode = code,
            BaseName = name ?? code,
        };
    }
}
