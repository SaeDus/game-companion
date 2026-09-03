import type { QuestLog, Difficulty, Act } from "../types/gameState";
import QuestCard from "./QuestCard";

interface QuestLogPanelProps {
  questLog: QuestLog;
}

const difficulties = [
  "Normal",
  "Nightmare",
  "Hell",
] as const;

const acts = [
  "ActI",
  "ActII",
  "ActIII",
  "ActIV",
  "ActV",
] as const;

const actLabels: Record<string, string> = {
  ActI: "Act I",
  ActII: "Act II",
  ActIII: "Act III",
  ActIV: "Act IV",
  ActV: "Act V",
};

function QuestLogPanel({ questLog }: QuestLogPanelProps) {
  return (
    <section>
      <h1>Quest Log</h1>

      {difficulties.map((difficultyName) => {
        const difficulty: Difficulty = questLog[difficultyName];

        return (
          <section key={difficultyName} className="quest-difficulty">
            <h2>{difficultyName}</h2>

            {acts.map((actName) => {
              const act: Act = difficulty[actName];

              return (
                <section key={actName} className="quest-act">
                  <h3>{actLabels[actName]}</h3>

                  <div className="quest-grid">
                    {act.Quests.map((quest) => (
                      <QuestCard
                        key={quest.Name}
                        quest={quest}
                      />
                    ))}
                  </div>
                </section>
              );
            })}
          </section>
        );
      })}
    </section>
  );
}

export default QuestLogPanel;
