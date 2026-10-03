using D2SSharp.Enums;
using D2SSharp.Model;

public class QuestLogFormatterTests
{
    [Theory]
    [InlineData(QuestFlags.RewardGranted)]
    [InlineData(QuestFlags.RewardPending)]
    [InlineData(QuestFlags.CompletedNow)]
    [InlineData(QuestFlags.CompletedBefore)]
    public void BuildQuestLogState_WhenQuestHasCompletionFlag_ReturnsCompleted(
        QuestFlags completionFlag
    )
    {
        QuestSection quests = new();
        quests.Normal.ActI.DenOfEvil = completionFlag;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        QuestState denOfEvil = Assert.Single(
            result.Normal!.ActI!.Quests!,
            quest => quest.Name == "The Den of Evil"
        );

        Assert.Equal("Completed", denOfEvil.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenNoQuestsStarted_ReturnsNotStarted()
    {
        QuestSection quests = new();

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("NotStarted", result.QuestLogStatus);
    }

    [Fact]
    public void BuildQuestLogState_WhenDenOfEvilIsInProgress_ExpandsActI()
    {
        QuestSection quests = new();
        quests.Normal.ActI.DenOfEvil = QuestFlags.Started;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.NotNull(result.Normal);
        Assert.NotNull(result.Normal.ActI);
        Assert.NotNull(result.Normal.ActI.Quests);

        QuestState denOfEvil = Assert.Single(
            result.Normal.ActI.Quests,
            quest => quest.Name == "The Den of Evil"
        );

        Assert.Equal("InProgress", denOfEvil.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenAndarielQuestIsComplete_CollapsesActI()
    {
        QuestSection quests = new();
        quests.Normal.ActI.SistersToTheSlaughter = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("Completed", result.Normal?.ActI?.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenDurielQuestIsComplete_CollapsesActII()
    {
        QuestSection quests = new();
        quests.Normal.ActII.TheSevenTombs = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("Completed", result.Normal?.ActII?.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenMephistoQuestIsComplete_CollapsesActIII()
    {
        QuestSection quests = new();
        quests.Normal.ActIII.TheGuardian = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("Completed", result.Normal?.ActIII?.Status);
    }

    [Fact]
    public void BuiltQuestLogState_WhenDiabloQuestIsComplete_CollapsesActIV()
    {
        QuestSection quests = new();
        quests.Normal.ActIV.TerrorsEnd = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("Completed", result.Normal?.ActIV?.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenBaalQuestIsComplete_CollapsesActV()
    {
        QuestSection quests = new();
        quests.Normal.ActV.EveOfDestruction = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("Completed", result.Normal?.ActV?.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenOnlyActIStarted_OtherActsAreNotStarted()
    {
        QuestSection quests = new();
        quests.Normal.ActI.DenOfEvil = QuestFlags.Started;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.Equal("NotStarted", result.Normal?.ActII?.Status);
        Assert.Equal("NotStarted", result.Normal?.ActIII?.Status);
        Assert.Equal("NotStarted", result.Normal?.ActIV?.Status);
        Assert.Equal("NotStarted", result.Normal?.ActV?.Status);
    }

    [Fact]
    public void BuildQuestLogState_WhenAllNormalActsComplete_CollapsesNormalDifficulty()
    {
        QuestSection quests = new();
        quests.Normal.ActI.SistersToTheSlaughter = QuestFlags.RewardGranted;
        quests.Normal.ActII.TheSevenTombs = QuestFlags.RewardGranted;
        quests.Normal.ActIII.TheGuardian = QuestFlags.RewardGranted;
        quests.Normal.ActIV.TerrorsEnd = QuestFlags.RewardGranted;
        quests.Normal.ActV.EveOfDestruction = QuestFlags.RewardGranted;

        QuestLogState result = QuestLogFormatter.BuildQuestLogState(quests);

        Assert.NotNull(result.Normal);
        Assert.Equal("Completed", result.Normal.DifficultyStatus);

        Assert.Null(result.Normal.ActI);
        Assert.Null(result.Normal.ActII);
        Assert.Null(result.Normal.ActIII);
        Assert.Null(result.Normal.ActIV);
        Assert.Null(result.Normal.ActV);
    }
}
