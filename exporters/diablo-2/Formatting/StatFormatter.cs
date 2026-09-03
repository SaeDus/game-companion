using System.Text;
using D2SSharp.Enums;
using D2SSharp.Model;

public static class StatFormatter
{
    private static readonly HashSet<StatId> IgnoredStats =
    [
        StatId.ColdLength,
        StatId.ColdMaxDamage,
        StatId.ExtraBlood,
        StatId.FireMaxDamage,
        StatId.LightningMaxDamage,
        StatId.MagicMaxDamage,
        StatId.MaxDamagePercent,
        StatId.MaxDurability,
        StatId.PoisonCount,
        StatId.PoisonLength,
        StatId.PoisonMaxDamage,
        StatId.QuestItemDifficulty,
        StatId.SecMaxDamage,
        StatId.SecondMinDamage,
        StatId.ThrowMaxDamage,
        StatId.ThrowMinDamage,
    ];

    private static readonly HashSet<StatId> UnhandledStats = [];

    public static bool TryFormatStat(Stat stat, IEnumerable<Stat> allStats, out string statText)
    {
        if (IgnoredStats.Contains(stat.Id))
        {
            statText = "";
            return false;
        }

        statText = FormatStat(stat, allStats);
        return true;
    }

    public static string GetUnhandledStats()
    {
        StringBuilder sb = new();

        sb.Append("\n****************************************\n");
        sb.Append("Unhandled Stat IDs:");

        foreach (var id in UnhandledStats.OrderBy(id => id))
        {
            sb.Append(id);
        }

        sb.Append("\n****************************************\n");

        return sb.ToString();
    }

    private static string FormatStat(Stat stat, IEnumerable<Stat> allStats)
    {
        return stat.Id switch
        {
            StatId.AddClassSkills =>
                $"+{stat.Value} to {PlayerClassLookup.GetName(stat.Layer)} Skill Levels",

            StatId.AddSkillTab => FormatSkillTab(stat),

            StatId.AllSkills => $"+{stat.Value} to All Skills",

            StatId.ArmorClass => $"+{stat.Value} Defense",

            StatId.ArmorPercent => $"+{stat.Value}% Enhanced Defense",

            StatId.AttackerTakesDamage => $"Attacker Takes Damage of {stat.Value}",

            StatId.AttackerTakesLightDamage => $"Attacker Takes Lightning Damage of {stat.Value}",

            StatId.AttackRating => $"+{stat.Value} to Attack Rating",

            StatId.AttackRatingPercent => $"{stat.Value}% Bonus to Attack Rating",

            StatId.Aura =>
                $"Level {stat.Value} {SkillLookup.GetName(stat.Layer)} Aura When Equipped",

            StatId.CannotBeFrozen => $"Cannot Be Frozen",

            StatId.ChanceToBlock => $"{stat.Value}% Increased Chance of Blocking",

            StatId.ColdMinDamage => FormatDamageRange(allStats, stat),

            StatId.ColdResist => $"Cold Resist +{stat.Value}%",

            StatId.CrushingBlow => $"{stat.Value}% Chance of Crushing Blow",

            StatId.DamageTakenGoesToMana => $"+{stat.Value}% Damage Taken Goes to Mana",

            StatId.DamageReduced => $"Physical Damage Received Reduced by {stat.Value}%",

            StatId.DeadlyStrike => $"{stat.Value}% Deadly Strike",

            StatId.Dexterity => $"+{stat.Value} to Dexterity",

            StatId.ElemSkills => $"+{stat.Value} to {ElemTypeLookup.GetName(stat.Layer)} Skills",

            StatId.EnemyColdResist => $"-{stat.Value}% to Enemy Cold Resistance",

            StatId.EnemyFireResist => $"-{stat.Value}% to Enemy Fire Resistance",

            StatId.EnemyLightningResist => $"-{stat.Value}% to Enemy Lightning Resistance",

            StatId.EnemyPoisonResist => $"-{stat.Value}% to Enemy Poison Resistance",

            StatId.Energy => $"+{stat.Value} to Energy",

            StatId.ExplosiveArrow => $"Fires Explosive Bolts",

            StatId.FasterBlockRate => $"{stat.Value}% Faster Block Rate",

            StatId.FasterCastRate => $"+{stat.Value}% Faster Cast Rate",

            StatId.FasterHitRecovery => $"+{stat.Value}% Faster Hit Recovery",

            StatId.FasterRunWalk => $"+{stat.Value}% Faster Run/Walk",

            StatId.FireMinDamage => FormatDamageRange(allStats, stat),

            StatId.FireResist => $"Fire Resist +{stat.Value}%",

            StatId.Freeze => $"Freezes Target",

            StatId.GoldFind => $"{stat.Value}% Extra Gold from Monsters",

            StatId.HalfFreezeDuration => $"Half Freeze Duration",

            StatId.HitPointRegeneration => $"Replenish Life +{stat.Value}",

            StatId.IncreasedAttackSpeed => $"+{stat.Value}% Increased Attack Speed",

            StatId.ItemChargedSkill => FormatChargedSkill(stat),

            StatId.LifeSteal => $"{stat.Value}% Life Stolen Per Hit",

            StatId.LightningMinDamage => FormatDamageRange(allStats, stat),

            StatId.LightningResist => $"Lightning Resist +{stat.Value}%",

            StatId.LightRadius => $"+{stat.Value} to Light Radius",

            StatId.MagicArrow => $"Fires Magic Arrows",

            StatId.MagicDamageReduction => $"Magic Damage Reduced by {stat.Value}",

            StatId.MagicFind => $"{stat.Value}% Better Chance of Getting Magic Items",

            StatId.MagicMinDamage => FormatDamageRange(allStats, stat),

            StatId.ManaAfterKill => $"+{stat.Value} to Mana After Each Kill",

            StatId.ManaRecoveryBonus => $"Regenerate Mana {stat.Value}%",

            StatId.ManaSteal => $"{stat.Value}% Mana Stolen Per Hit",

            StatId.MaxColdResist => $"+{stat.Value}% to Maximum Cold Resist",

            StatId.MaxDamage => $"+{stat.Value} to Maximum Damage",

            StatId.MaxDamagePerLevel =>
                $"+{stat.Value} to Maximum Damage (Based on Character Level)",

            StatId.MaxDurabilityPercent => $"Increase Maximum Durability {stat.Value}%",

            StatId.MaxFireResist => $"+{stat.Value}% to Maximum Fire Resist",

            StatId.MaxHitPointPercent => $"Increase Maximum Life {stat.Value}%",

            StatId.MaxLife => $"+{stat.Value / 256} to Life",

            StatId.MaxLightningResist => $"+{stat.Value}% to Maximum Lightning Resist",

            StatId.MaxMagicResist => $"+{stat.Value}% to Maximum Magic Resist",

            StatId.MaxMana => $"+{stat.Value / 256} to Mana",

            StatId.MaxPoisonResist => $"+{stat.Value}% to Maximum Poison Resist",

            StatId.MaxStamina => $"+{stat.Value / 256} Maximum Stamina",

            StatId.MinDamage => $"+{stat.Value} to Minimum Damage",

            StatId.MinDamagePercent => $"+{stat.Value}% Enhanced Damage",

            StatId.NormalDamageReduction => $"Damage Reduced by {stat.Value}",

            StatId.NonClassSkill => $"+{stat.Value} to {SkillLookup.GetName(stat.Layer)}",

            StatId.OpenWounds => $"{stat.Value}% Chance of Open Wounds",

            StatId.PoisonLengthResist => $"Poison Length Reduced by {stat.Value}%",

            StatId.PoisonMinDamage => FormatPoisonDamage(allStats),

            StatId.PoisonResist => $"Poison Resist +{stat.Value}%",

            StatId.ReplenishDurability => $"Repairs 1 Durability in {100 / stat.Value} Seconds",

            StatId.SecMaxDamage => $"+{stat.Value} to Maximum Damage",

            StatId.SingleSkill => $"+{stat.Value} to {SkillLookup.GetName(stat.Layer)}",

            StatId.SkillOnAttack => FormatSkillStat(stat, "On Attack"),

            StatId.SkillOnDeath => FormatSkillStat(stat, "When You Die"),

            StatId.SkillOnGetHit => FormatSkillStat(stat, "When Struck"),

            StatId.SkillOnHit => FormatSkillStat(stat, "On Striking"),

            StatId.SkillOnKill => FormatSkillStat(stat, "When You Kill An Enemy"),

            StatId.SkillOnLevelUp => FormatSkillStat(stat, "When You Level Up"),

            StatId.Slow => $"Slows Target By {stat.Value}%",

            StatId.Strength => $"+{stat.Value} to Strength",

            StatId.Vitality => $"+{stat.Value} to Vitality",

            _ => FormatUnhandledStat(stat),
        };
    }

    private static string FormatDamageRange(IEnumerable<Stat> stats, Stat minStat)
    {
        StatId matchingType;
        string damageType;

        switch (minStat.Id)
        {
            case StatId.ColdMinDamage:
                matchingType = StatId.ColdMaxDamage;
                damageType = "Cold Damage";
                break;
            case StatId.FireMinDamage:
                matchingType = StatId.FireMaxDamage;
                damageType = "Fire Damage";
                break;
            case StatId.LightningMinDamage:
                matchingType = StatId.LightningMaxDamage;
                damageType = "Lightning Damage";
                break;
            default:
                matchingType = StatId.MagicMaxDamage;
                damageType = "Magic Damage";
                break;
        }

        Stat? maxStat = stats.FirstOrDefault(s => s.Id == matchingType);

        string damage =
            minStat.Value == maxStat?.Value
                ? $"{minStat.Value}"
                : $"{minStat.Value}-{maxStat?.Value}";

        return $"Adds {damage} {damageType}";
    }

    private static string FormatChargedSkill(Stat stat)
    {
        (int skillId, int skillLevel) = DecodeSkillLayer(stat.Layer);

        int currentCharges = (int)(stat.Value & 0xFF);
        int maxCharges = (int)((stat.Value >> 8) & 0xFF);

        string skillName = SkillLookup.GetName(skillId);

        return $"Level {skillLevel} {skillName} ({currentCharges}/{maxCharges} Charges)";
    }

    private static string FormatPoisonDamage(IEnumerable<Stat> stats)
    {
        Stat? poisonMin = stats.FirstOrDefault(s => s.Id == StatId.PoisonMinDamage);
        Stat? poisonMax = stats.FirstOrDefault(s => s.Id == StatId.PoisonMaxDamage);
        Stat? poisonLength = stats.FirstOrDefault(s => s.Id == StatId.PoisonLength);

        if (poisonMin == null || poisonMax == null || poisonLength == null)
        {
            return "Error - unable to access poison damage";
        }

        double seconds = poisonLength.Value / 25.0;

        double totalMin = poisonMin.Value * poisonLength.Value / 256.0;
        double totalMax = poisonMax.Value * poisonLength.Value / 256.0;

        int displayMin = (int)Math.Round(totalMin);
        int displayMax = (int)Math.Round(totalMax);

        string duration = seconds % 1 == 0 ? ((int)seconds).ToString() : seconds.ToString("0.##");

        string damage = displayMin == displayMax ? $"{displayMin}" : $"{displayMin}-{displayMax}";

        return $"+{damage} Poison damage over {duration} seconds";
    }

    private static string FormatSkillStat(Stat stat, string condition)
    {
        (int id, int level) = DecodeSkillLayer(stat.Layer);
        string skill = SkillLookup.GetName(id);

        return $"{stat.Value}% Chance to Cast Level {level} {skill} {condition}";
    }

    private static string FormatSkillTab(Stat stat)
    {
        string skillClass = SkillTabLookup.GetClass(stat.Layer);
        string skillTree = SkillTabLookup.GetTree(stat.Layer);

        return $"+{stat.Value} to {skillTree} Skills ({skillClass} Only)";
    }

    private static (int SkillId, int SkillLevel) DecodeSkillLayer(int layer)
    {
        int skillLevel = layer & 0x3F;
        int skillId = layer >> 6;

        return (skillId, skillLevel);
    }

    private static string FormatUnhandledStat(Stat stat)
    {
        UnhandledStats.Add(stat.Id);
        return $"[UNHANDLED] > {stat.Id}: {stat.Value}";
    }
}
