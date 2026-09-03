using System.Text.Json;

public static class WeaponLookup
{
    private static readonly Dictionary<string, WeaponMetadata> _weaponDatabase;

    static WeaponLookup()
    {
        string path = Path.Combine(Data.GetDataDirectory(), "weapons.json");

        string json = File.ReadAllText(path);

        _weaponDatabase =
            JsonSerializer
                .Deserialize<GeneratedJson<Dictionary<string, WeaponMetadata>>>(json)
                ?.Data
            ?? [];
    }

    public static bool TryGetName(string itemId, out string weaponName)
    {
        if (!_weaponDatabase.TryGetValue(itemId, out WeaponMetadata? weapon))
        {
            weaponName = "";
            return false;
        }

        weaponName = weapon.Name;
        return true;
    }
}
