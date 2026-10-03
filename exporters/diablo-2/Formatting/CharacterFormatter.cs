using D2SSharp.Enums;
using D2SSharp.Model;

public static class CharacterFormatter
{
    public static CharacterState BuildCharacterState(D2Save save)
    {
        CharacterState characterState = new()
        {
            Name = save.Character.Preview.Name,
            Level = save.Character.Level,
            Class = save.Character.Class.ToString(),

            BaseAttributes = BuildAttributeState(save.Stats),
            UnspentStatPoints = (int)save.Stats.GetStat(StatId.StatPoints),

            BaseSkills = [.. BuildSkillState(save.Skills)],
            UnspentSkillPoints = (int)save.Stats.GetStat(StatId.SkillPoints),
        };

        foreach (var item in BuildItemStates(save.Items))
        {
            if (item.Slot != null)
            {
                characterState.Equipment.Add(item);
                continue;
            }

            if ((item.Flags & ItemFlags.InStore) != 0)
            {
                characterState.PersonalStash.Add(item);
            }
            else
            {
                characterState.Inventory.Add(item);
            }
        }

        return characterState;
    }

    public static MercenaryState BuildMercenaryState(MercData mercData, MercItemsSection? mercItems)
    {
        HirelingMetadata? hireling = HirelingLookup.Get(mercData.HirelingId);

        MercenaryState mercenaryState = new()
        {
            Id = mercData.HirelingId,
            NameIndex = mercData.NameIndex,
            Experience = mercData.Experience,
        };

        if (hireling == null)
        {
            return mercenaryState;
        }

        mercenaryState.Hireling = hireling.Identity.Hireling;
        mercenaryState.SubType = hireling.Identity.SubType;

        HirelingResolvedState resolved = GetHirelingLevel(
            mercData.Experience,
            hireling.Rows
        );

        mercenaryState.Level = resolved.Level;

        mercenaryState.Strength = CalculateEighthStat(
            resolved.Level,
            resolved.Row.Progression.Level,
            resolved.Row.Stats.Strength.Base,
            resolved.Row.Stats.Strength.PerLevel
        );

        mercenaryState.Dexterity = CalculateEighthStat(
            resolved.Level,
            resolved.Row.Progression.Level,
            resolved.Row.Stats.Dexterity.Base,
            resolved.Row.Stats.Dexterity.PerLevel
        );

        mercenaryState.Skills = [];

        foreach (var skill in resolved.Row.Skills)
        {
            mercenaryState.Skills.Add(skill.Name);
        }

        if (mercItems == null)
        {
            return mercenaryState;
        }

        foreach (var item in BuildItemStates(mercItems.Items))
        {
            mercenaryState.Equipment.Add(item);
        }

        return mercenaryState;
    }

    public static StashState BuildStashState(D2StashSave stash)
    {
        StashState stashState = new();

        int i = 1;

        foreach (var tab in stash)
        {
            StashTabState stashTabState = new()
            {
                TabName = i switch
                {
                    6 => "Runes and Gems",
                    7 => "Materials",
                    _ => $"Shared Tab {i}",
                },

                Items = [.. BuildItemStates(tab.Items)],
            };

            stashState.Tabs.Add(stashTabState);

            i++;
        }

        return stashState;
    }

    public static IEnumerable<ItemState> BuildItemStates(ItemsSection items)
    {
        foreach (Item item in items)
        {
            if (!ItemFormatter.TryBuildItem(item, false, out var itemState))
            {
                continue;
            }

            yield return itemState;
        }
    }

    private static HirelingResolvedState GetHirelingLevel(
        uint experience,
        IReadOnlyList<HirelingRow> rows
    )
    {
        var expansionRows = rows.Where(row => row.Version == 100)
            .OrderBy(row => row.Progression.Level)
            .ToList();

        int level = expansionRows[0].Progression.Level;
        HirelingRow row = new();

        for (int candidateLevel = level; candidateLevel <= 98; candidateLevel++)
        {
            row = expansionRows.Last(row => row.Progression.Level <= candidateLevel);

            ulong requiredExperience =
                (ulong)row.Progression.ExperiencePerLevel
                * (ulong)(candidateLevel + 1)
                * (ulong)candidateLevel
                * (ulong)candidateLevel;

            if ((ulong)experience < requiredExperience)
                break;

            level = candidateLevel;
        }

        return new HirelingResolvedState() { Level = level, Row = row };
    }

    private static int CalculateEighthStat(int mercLevel, int rowLevel, int baseValue, int perLevel)
    {
        int levelsGained = mercLevel - rowLevel;

        return baseValue + (levelsGained * perLevel / 8);
    }

    private static AttributeState BuildAttributeState(PlayerStats stats)
    {
        return new AttributeState()
        {
            Strength = (int)stats.GetStat(StatId.Strength),
            Dexterity = (int)stats.GetStat(StatId.Dexterity),
            Vitality = (int)stats.GetStat(StatId.Vitality),
            Energy = (int)stats.GetStat(StatId.Energy),
        };
    }

    private static IEnumerable<SkillState> BuildSkillState(SkillsSection skills)
    {
        for (int i = 0; i < 30; i++)
        {
            int points = skills[i];

            if (points == 0)
            {
                continue;
            }

            int skillId = skills.GetSkillId(i);

            yield return new SkillState()
            {
                Id = skillId,
                Name = SkillLookup.GetName(skillId),
                Level = points,
            };
        }
    }
}
