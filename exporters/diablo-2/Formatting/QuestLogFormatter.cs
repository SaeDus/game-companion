using D2SSharp.Enums;
using D2SSharp.Model;

public static class QuestLogFormatter {
    public static QuestLogState BuildQuestLogState(D2Save save) {
        QuestLogState questLogState = new() {
            Normal = BuildDifficultyState(save.Quests.Normal),
            Nightmare = BuildDifficultyState(save.Quests.Nightmare),
            Hell = BuildDifficultyState(save.Quests.Hell),
        };

        return questLogState;
    }
    
    private static DifficultyState BuildDifficultyState(QuestsDifficulty difficulty) {
        DifficultyState difficultyState = new() {
            ActI = BuildActIState(difficulty.ActI),
            ActII = BuildActIIState(difficulty.ActII),
            ActIII = BuildActIIIState(difficulty.ActIII),
            ActIV = BuildActIVState(difficulty.ActIV),
            ActV = BuildActVState(difficulty.ActV),
        };

        return difficultyState;
    }

    private static ActState BuildActIState(ActIQuests quests) {
        ActState actIState = new();

        actIState.Quests.Add(new() {
            Name = "The Den of Evil",
            IsOptional = true,
            State = GetQuestState(quests.DenOfEvil),
            Status = quests.DenOfEvil.ToString()
        });

        actIState.Quests.Add(new() {
            Name = "Sisters' Burial Grounds",
            IsOptional = true,
            State = GetQuestState(quests.SistersBurialGrounds),
            Status = quests.SistersBurialGrounds.ToString()
        });

        actIState.Quests.Add(new() {
            Name = "The Search for Cain",
            IsOptional = true,
            State = GetQuestState(quests.TheSearchForCain),
            Status = quests.TheSearchForCain.ToString()
        });

        actIState.Quests.Add(new() {
            Name = "The Forgotten Tower",
            IsOptional = true,
            State = GetQuestState(quests.TheForgottenTower),
            Status = quests.TheForgottenTower.ToString()
        });

        actIState.Quests.Add(new() {
            Name = "Tools of the Trade",
            IsOptional = true,
            State = GetQuestState(quests.ToolsOfTheTrade),
            Status = quests.ToolsOfTheTrade.ToString()
        });

        actIState.Quests.Add(new() {
            Name = "Sisters to the Slaughter",
            IsOptional = false,
            State = GetQuestState(quests.SistersToTheSlaughter),
            Status = quests.SistersToTheSlaughter.ToString()
        });

        return actIState;
    }

    private static ActState BuildActIIState(ActIIQuests quests) {
        ActState actIIState = new();

        actIIState.Quests.Add(new() {
            Name = "Radament's Lair",
            IsOptional = true,
            State = GetQuestState(quests.RadamentsLair),
            Status = quests.RadamentsLair.ToString()
        });

        actIIState.Quests.Add(new() {
            Name = "The Horadric Staff",
            IsOptional = false,
            State = GetQuestState(quests.TheHoradricStaff),
            Status = quests.TheHoradricStaff.ToString()
        });

        actIIState.Quests.Add(new() {
            Name = "The Tainted Sun",
            IsOptional = false,
            State = GetQuestState(quests.TaintedSun),
            Status = quests.TaintedSun.ToString()
        });

        actIIState.Quests.Add(new() {
            Name = "Arcane Sanctuary",
            IsOptional = false,
            State = GetQuestState(quests.ArcaneSanctuary),
            Status = quests.ArcaneSanctuary.ToString()
        });

        actIIState.Quests.Add(new() {
            Name = "The Summoner",
            IsOptional = false,
            State = GetQuestState(quests.TheSummoner),
            Status = quests.TheSummoner.ToString()
        });

        actIIState.Quests.Add(new() {
            Name = "The Seven Tombs",
            IsOptional = false,
            State = GetQuestState(quests.TheSevenTombs),
            Status = quests.TheSevenTombs.ToString()
        });

        return actIIState;
    }

    private static ActState BuildActIIIState(ActIIIQuests quests) {
        ActState actIIIState = new();

        actIIIState.Quests.Add(new() {
            Name = "The Golden Bird",
            IsOptional = true,
            State = GetQuestState(quests.TheGoldenBird),
            Status = quests.TheGoldenBird.ToString()
        });

        actIIIState.Quests.Add(new() {
            Name = "Blade of the Old Religion",
            IsOptional = true,
            State = GetQuestState(quests.BladeOfTheOldReligion),
            Status = quests.BladeOfTheOldReligion.ToString()
        });

        actIIIState.Quests.Add(new() {
            Name = "Khalim's Will",
            IsOptional = false,
            State = GetQuestState(quests.KhalimsWill),
            Status = quests.KhalimsWill.ToString()
        });

        actIIIState.Quests.Add(new() {
            Name = "Lam Esen's Tome",
            IsOptional = true,
            State = GetQuestState(quests.LamEsensTome),
            Status = quests.LamEsensTome.ToString()
        });

        actIIIState.Quests.Add(new() {
            Name = "The Blackened Temple",
            IsOptional = false,
            State = GetQuestState(quests.TheBlackenedTemple),
            Status = quests.TheBlackenedTemple.ToString()
        });

        actIIIState.Quests.Add(new() {
            Name = "The Guardian",
            IsOptional = false,
            State = GetQuestState(quests.TheGuardian),
            Status = quests.TheGuardian.ToString()
        });

        return actIIIState;
    }

    private static ActState BuildActIVState(ActIVQuests quests) {
        ActState actIVState = new();

        actIVState.Quests.Add(new() {
            Name = "Fallen Angel",
            IsOptional = true,
            State = GetQuestState(quests.TheFallenAngel),
            Status = quests.TheFallenAngel.ToString()
        });

        actIVState.Quests.Add(new() {
            Name = "Hell's Forge",
            IsOptional = true,
            State = GetQuestState(quests.Hellforge),
            Status = quests.Hellforge.ToString()
        });

        actIVState.Quests.Add(new() {
            Name = "Terror's End",
            IsOptional = false,
            State = GetQuestState(quests.TerrorsEnd),
            Status = quests.TerrorsEnd.ToString()
        });

        return actIVState;
    }

    private static ActState BuildActVState(ActVQuests quests) {
        ActState actVState = new();

        actVState.Quests.Add(new() {
            Name = "Siege on Harrogath",
            IsOptional = true,
            State = GetQuestState(quests.SiegeOnHarrogath),
            Status = quests.SiegeOnHarrogath.ToString()
        });

        actVState.Quests.Add(new() {
            Name = "Rescue on Mount Arreat",
            IsOptional = true,
            State = GetQuestState(quests.RescueOnMountArreat),
            Status = quests.RescueOnMountArreat.ToString()
        });

        actVState.Quests.Add(new() {
            Name = "Prison of Ice",
            IsOptional = true,
            State = GetQuestState(quests.PrisonOfIce),
            Status = quests.PrisonOfIce.ToString()
        });

        actVState.Quests.Add(new() {
            Name = "Betrayal of Harrogath",
            IsOptional = true,
            State = GetQuestState(quests.BetrayalOfHarrogath),
            Status = quests.BetrayalOfHarrogath.ToString()
        });

        actVState.Quests.Add(new() {
            Name = "Rite of Passage",
            IsOptional = true,
            State = GetQuestState(quests.RiteOfPassage),
            Status = quests.RiteOfPassage.ToString()
        });

        actVState.Quests.Add(new() {
            Name = "Eve of Destruction",
            IsOptional = false,
            State = GetQuestState(quests.EveOfDestruction),
            Status = quests.EveOfDestruction.ToString()
        });

        return actVState;
    }

    private static string GetQuestState(QuestFlags flags)
    {
        if (flags == QuestFlags.None)
            return "NotStarted";

        QuestFlags completedFlags =
            QuestFlags.RewardGranted |
            QuestFlags.RewardPending |
            QuestFlags.CompletedNow |
            QuestFlags.CompletedBefore;

        if ((flags & completedFlags) != 0)
            return "Completed";

        return "InProgress";
    }
}