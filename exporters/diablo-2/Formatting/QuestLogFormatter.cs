using D2SSharp.Enums;
using D2SSharp.Model;

public static class QuestLogFormatter
{
    public static QuestLogState BuildQuestLogState(D2Save save)
    {
        DifficultyState normal = BuildDifficultyState(save.Quests.Normal);
        DifficultyState nightmare = BuildDifficultyState(save.Quests.Nightmare);
        DifficultyState hell = BuildDifficultyState(save.Quests.Hell);

        if (
            normal.DifficultyStatus is "Completed"
            && nightmare.DifficultyStatus is "Completed"
            && hell.DifficultyStatus is "Completed"
        )
        {
            return new() { QuestLogStatus = "Completed" };
        }

        QuestLogState questLogState = new()
        {
            Normal = normal,
            Nightmare = nightmare,
            Hell = hell,
        };

        return questLogState;
    }

    private static DifficultyState BuildDifficultyState(QuestsDifficulty difficulty)
    {
        ActState actI = BuildActIState(difficulty.ActI);
        ActState actII = BuildActIIState(difficulty.ActII);
        ActState actIII = BuildActIIIState(difficulty.ActIII);
        ActState actIV = BuildActIVState(difficulty.ActIV);
        ActState actV = BuildActVState(difficulty.ActV);

        if (
            actI.Status is "Completed"
            && actII.Status is "Completed"
            && actIII.Status is "Completed"
            && actIV.Status is "Completed"
            && actV.Status is "Completed"
        )
        {
            return new() { DifficultyStatus = "Completed" };
        }

        DifficultyState difficultyState = new()
        {
            ActI = actI,
            ActII = actII,
            ActIII = actIII,
            ActIV = actIV,
            ActV = actV,
        };

        return difficultyState;
    }

    private static ActState BuildActIState(ActIQuests quests)
    {
        ActState actIState = new();

        if (GetQuestState(quests.SistersToTheSlaughter) == "Completed")
        {
            actIState.Status = "Completed";
            return actIState;
        }

        actIState.Quests = [];

        actIState.Quests.Add(
            new()
            {
                Name = "The Den of Evil",
                IsOptional = true,
                Status = GetQuestState(quests.DenOfEvil),
                Flags = quests.DenOfEvil.ToString(),
            }
        );

        actIState.Quests.Add(
            new()
            {
                Name = "Sisters' Burial Grounds",
                IsOptional = true,
                Status = GetQuestState(quests.SistersBurialGrounds),
                Flags = quests.SistersBurialGrounds.ToString(),
            }
        );

        actIState.Quests.Add(
            new()
            {
                Name = "The Search for Cain",
                IsOptional = true,
                Status = GetQuestState(quests.TheSearchForCain),
                Flags = quests.TheSearchForCain.ToString(),
            }
        );

        actIState.Quests.Add(
            new()
            {
                Name = "The Forgotten Tower",
                IsOptional = true,
                Status = GetQuestState(quests.TheForgottenTower),
                Flags = quests.TheForgottenTower.ToString(),
            }
        );

        actIState.Quests.Add(
            new()
            {
                Name = "Tools of the Trade",
                IsOptional = true,
                Status = GetQuestState(quests.ToolsOfTheTrade),
                Flags = quests.ToolsOfTheTrade.ToString(),
            }
        );

        actIState.Quests.Add(
            new()
            {
                Name = "Sisters to the Slaughter",
                IsOptional = false,
                Status = GetQuestState(quests.SistersToTheSlaughter),
                Flags = quests.SistersToTheSlaughter.ToString(),
            }
        );

        return actIState;
    }

    private static ActState BuildActIIState(ActIIQuests quests)
    {
        ActState actIIState = new();

        if (GetQuestState(quests.TheSevenTombs) == "Completed")
        {
            actIIState.Status = "Completed";
            return actIIState;
        }

        actIIState.Quests = [];

        actIIState.Quests.Add(
            new()
            {
                Name = "Radament's Lair",
                IsOptional = true,
                Status = GetQuestState(quests.RadamentsLair),
                Flags = quests.RadamentsLair.ToString(),
            }
        );

        actIIState.Quests.Add(
            new()
            {
                Name = "The Horadric Staff",
                IsOptional = false,
                Status = GetQuestState(quests.TheHoradricStaff),
                Flags = quests.TheHoradricStaff.ToString(),
            }
        );

        actIIState.Quests.Add(
            new()
            {
                Name = "The Tainted Sun",
                IsOptional = false,
                Status = GetQuestState(quests.TaintedSun),
                Flags = quests.TaintedSun.ToString(),
            }
        );

        actIIState.Quests.Add(
            new()
            {
                Name = "Arcane Sanctuary",
                IsOptional = false,
                Status = GetQuestState(quests.ArcaneSanctuary),
                Flags = quests.ArcaneSanctuary.ToString(),
            }
        );

        actIIState.Quests.Add(
            new()
            {
                Name = "The Summoner",
                IsOptional = false,
                Status = GetQuestState(quests.TheSummoner),
                Flags = quests.TheSummoner.ToString(),
            }
        );

        actIIState.Quests.Add(
            new()
            {
                Name = "The Seven Tombs",
                IsOptional = false,
                Status = GetQuestState(quests.TheSevenTombs),
                Flags = quests.TheSevenTombs.ToString(),
            }
        );

        return actIIState;
    }

    private static ActState BuildActIIIState(ActIIIQuests quests)
    {
        ActState actIIIState = new();

        if (GetQuestState(quests.TheGuardian) == "Completed")
        {
            actIIIState.Status = "Completed";
            return actIIIState;
        }

        actIIIState.Quests = [];

        actIIIState.Quests.Add(
            new()
            {
                Name = "The Golden Bird",
                IsOptional = true,
                Status = GetQuestState(quests.TheGoldenBird),
                Flags = quests.TheGoldenBird.ToString(),
            }
        );

        actIIIState.Quests.Add(
            new()
            {
                Name = "Blade of the Old Religion",
                IsOptional = true,
                Status = GetQuestState(quests.BladeOfTheOldReligion),
                Flags = quests.BladeOfTheOldReligion.ToString(),
            }
        );

        actIIIState.Quests.Add(
            new()
            {
                Name = "Khalim's Will",
                IsOptional = false,
                Status = GetQuestState(quests.KhalimsWill),
                Flags = quests.KhalimsWill.ToString(),
            }
        );

        actIIIState.Quests.Add(
            new()
            {
                Name = "Lam Esen's Tome",
                IsOptional = true,
                Status = GetQuestState(quests.LamEsensTome),
                Flags = quests.LamEsensTome.ToString(),
            }
        );

        actIIIState.Quests.Add(
            new()
            {
                Name = "The Blackened Temple",
                IsOptional = false,
                Status = GetQuestState(quests.TheBlackenedTemple),
                Flags = quests.TheBlackenedTemple.ToString(),
            }
        );

        actIIIState.Quests.Add(
            new()
            {
                Name = "The Guardian",
                IsOptional = false,
                Status = GetQuestState(quests.TheGuardian),
                Flags = quests.TheGuardian.ToString(),
            }
        );

        return actIIIState;
    }

    private static ActState BuildActIVState(ActIVQuests quests)
    {
        ActState actIVState = new();

        if (GetQuestState(quests.TerrorsEnd) == "Completed")
        {
            actIVState.Status = "Completed";
            return actIVState;
        }

        actIVState.Quests = [];

        actIVState.Quests.Add(
            new()
            {
                Name = "Fallen Angel",
                IsOptional = true,
                Status = GetQuestState(quests.TheFallenAngel),
                Flags = quests.TheFallenAngel.ToString(),
            }
        );

        actIVState.Quests.Add(
            new()
            {
                Name = "Hell's Forge",
                IsOptional = true,
                Status = GetQuestState(quests.Hellforge),
                Flags = quests.Hellforge.ToString(),
            }
        );

        actIVState.Quests.Add(
            new()
            {
                Name = "Terror's End",
                IsOptional = false,
                Status = GetQuestState(quests.TerrorsEnd),
                Flags = quests.TerrorsEnd.ToString(),
            }
        );

        return actIVState;
    }

    private static ActState BuildActVState(ActVQuests quests)
    {
        ActState actVState = new();

        if (GetQuestState(quests.EveOfDestruction) == "Completed")
        {
            actVState.Status = "Completed";
            return actVState;
        }

        actVState.Quests = [];

        actVState.Quests.Add(
            new()
            {
                Name = "Siege on Harrogath",
                IsOptional = true,
                Status = GetQuestState(quests.SiegeOnHarrogath),
                Flags = quests.SiegeOnHarrogath.ToString(),
            }
        );

        actVState.Quests.Add(
            new()
            {
                Name = "Rescue on Mount Arreat",
                IsOptional = true,
                Status = GetQuestState(quests.RescueOnMountArreat),
                Flags = quests.RescueOnMountArreat.ToString(),
            }
        );

        actVState.Quests.Add(
            new()
            {
                Name = "Prison of Ice",
                IsOptional = true,
                Status = GetQuestState(quests.PrisonOfIce),
                Flags = quests.PrisonOfIce.ToString(),
            }
        );

        actVState.Quests.Add(
            new()
            {
                Name = "Betrayal of Harrogath",
                IsOptional = true,
                Status = GetQuestState(quests.BetrayalOfHarrogath),
                Flags = quests.BetrayalOfHarrogath.ToString(),
            }
        );

        actVState.Quests.Add(
            new()
            {
                Name = "Rite of Passage",
                IsOptional = true,
                Status = GetQuestState(quests.RiteOfPassage),
                Flags = quests.RiteOfPassage.ToString(),
            }
        );

        actVState.Quests.Add(
            new()
            {
                Name = "Eve of Destruction",
                IsOptional = false,
                Status = GetQuestState(quests.EveOfDestruction),
                Flags = quests.EveOfDestruction.ToString(),
            }
        );

        return actVState;
    }

    private static string GetQuestState(QuestFlags flags)
    {
        if (flags == QuestFlags.None)
            return "NotStarted";

        QuestFlags completedFlags =
            QuestFlags.RewardGranted
            | QuestFlags.RewardPending
            | QuestFlags.CompletedNow
            | QuestFlags.CompletedBefore;

        if ((flags & completedFlags) != 0)
            return "Completed";

        return "InProgress";
    }
}
