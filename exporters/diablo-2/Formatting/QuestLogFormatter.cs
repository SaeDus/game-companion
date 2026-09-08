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
            normal.DifficultyStatus is "NotStarted"
            && nightmare.DifficultyStatus is "NotStarted"
            && hell.DifficultyStatus is "NotStarted"
        )
        {
            return new() { QuestLogStatus = "NotStarted" };
        }

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
        DifficultyState difficultyState = new()
        {
            ActI = BuildActIState(difficulty.ActI),
            ActII = BuildActIIState(difficulty.ActII),
            ActIII = BuildActIIIState(difficulty.ActIII),
            ActIV = BuildActIVState(difficulty.ActIV),
            ActV = BuildActVState(difficulty.ActV),
        };

        CheckDifficultyStatus(difficultyState);

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

        return HasActBeenStarted(actIState.Quests) ? actIState : new() { Status = "NotStarted" };
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

        return HasActBeenStarted(actIIState.Quests) ? actIIState : new() { Status = "NotStarted" };
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

        return HasActBeenStarted(actIIIState.Quests)
            ? actIIIState
            : new() { Status = "NotStarted" };
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

        return HasActBeenStarted(actIVState.Quests) ? actIVState : new() { Status = "NotStarted" };
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

        return HasActBeenStarted(actVState.Quests) ? actVState : new() { Status = "NotStarted" };
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

    private static bool HasActBeenStarted(List<QuestState> quests)
    {
        foreach (var quest in quests)
        {
            if (quest.Status != "NotStarted")
            {
                return true;
            }
        }

        return false;
    }

    private static void CheckDifficultyStatus(DifficultyState difficulty)
    {
        ActState?[] acts =
        [
            difficulty.ActI,
            difficulty.ActII,
            difficulty.ActIII,
            difficulty.ActIV,
            difficulty.ActV,
        ];

        string? status = acts[0]?.Status;

        if (status is not ("Completed" or "NotStarted"))
            return;

        if (!acts.All(act => act?.Status == status))
            return;

        difficulty.DifficultyStatus = status;

        difficulty.ActI = null;
        difficulty.ActII = null;
        difficulty.ActIII = null;
        difficulty.ActIV = null;
        difficulty.ActV = null;
    }
}
