using System.Text.Json.Serialization;

public class HirelingMetadata
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("identity")]
    public HirelingIdentity Identity { get; set; } = new();

    [JsonPropertyName("rows")]
    public List<HirelingRow> Rows { get; set; } = [];
}


public class HirelingIdentity
{
    [JsonPropertyName("class")]
    public int Class { get; set; }

    [JsonPropertyName("act")]
    public int Act { get; set; }

    [JsonPropertyName("difficulty")]
    public int Difficulty { get; set; }

    [JsonPropertyName("hireling")]
    public string Hireling { get; set; } = "";

    [JsonPropertyName("subType")]
    public string SubType { get; set; } = "";
}


public class HirelingRow
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("progression")]
    public HirelingProgression Progression { get; set; } = new();

    [JsonPropertyName("stats")]
    public HirelingStats Stats { get; set; } = new();

    [JsonPropertyName("resistances")]
    public HirelingResistances Resistances { get; set; } = new();

    [JsonPropertyName("skills")]
    public List<HirelingSkill> Skills { get; set; } = [];
}


public class HirelingProgression
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("experiencePerLevel")]
    public int ExperiencePerLevel { get; set; }
}


public class HirelingStats
{
    [JsonPropertyName("hitPoints")]
    public HirelingScalingStat HitPoints { get; set; } = new();

    [JsonPropertyName("defense")]
    public HirelingScalingStat Defense { get; set; } = new();

    [JsonPropertyName("strength")]
    public HirelingScalingStat Strength { get; set; } = new();

    [JsonPropertyName("dexterity")]
    public HirelingScalingStat Dexterity { get; set; } = new();

    [JsonPropertyName("attackRating")]
    public HirelingScalingStat AttackRating { get; set; } = new();

    [JsonPropertyName("damage")]
    public HirelingDamage Damage { get; set; } = new();
}


public class HirelingScalingStat
{
    [JsonPropertyName("base")]
    public int Base { get; set; }

    [JsonPropertyName("perLevel")]
    public int PerLevel { get; set; }
}


public class HirelingDamage
{
    [JsonPropertyName("min")]
    public int Min { get; set; }

    [JsonPropertyName("max")]
    public int Max { get; set; }

    [JsonPropertyName("perLevel")]
    public int PerLevel { get; set; }
}


public class HirelingResistances
{
    [JsonPropertyName("fire")]
    public HirelingScalingStat Fire { get; set; } = new();

    [JsonPropertyName("cold")]
    public HirelingScalingStat Cold { get; set; } = new();

    [JsonPropertyName("lightning")]
    public HirelingScalingStat Lightning { get; set; } = new();

    [JsonPropertyName("poison")]
    public HirelingScalingStat Poison { get; set; } = new();
}


public class HirelingSkill
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("mode")]
    public int Mode { get; set; }

    [JsonPropertyName("chance")]
    public int Chance { get; set; }

    [JsonPropertyName("chancePerLevel")]
    public int ChancePerLevel { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("levelPerLevel")]
    public int LevelPerLevel { get; set; }
}