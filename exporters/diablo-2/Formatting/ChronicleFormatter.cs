using D2SSharp.Enums;
using D2SSharp.Model;

public static class ChronicleFormatter {
    public static ChronicleState BuildChronicleState(D2StashSave save) {
        ChronicleState chronicleState = new();

        foreach (var tab in save) {
            if (tab.Chronicle == null) {
                continue;
            }

            foreach (var uniqueEntry in tab.Chronicle.UniqueEntries) {
                ChronicleMetadata u = new() {
                    Name = UniqueLookup.GetName((int)uniqueEntry.ItemId),
                    Source = $"ID: [{uniqueEntry.Source}]",
                    Time = $"[{uniqueEntry.Timestamp}]"
                };

                chronicleState.UniqueEntries.Add(u);
            }

            foreach (var setEntry in tab.Chronicle.SetEntries) {
                ChronicleMetadata s = new() {
                    Name = SetItemLookup.GetName((int)setEntry.ItemId),
                    Source = $"ID: [{setEntry.Source}]",
                    Time = $"[{setEntry.Timestamp}]"
                };

                chronicleState.SetEntries.Add(s);
            }

            // foreach (var runewordEntry in tab.Chronicle.RunewordEntries) {
            //     ChronicleMetadata r = new() {
            //         Name = $"ItemID: [{runewordEntry.ItemId}]",
            //         Time = $"[{runewordEntry.Timestamp}]"
            //     };

            //     chronicleState.RunewordEntries.Add(r);
            // }
        }

        return chronicleState;
    }
}