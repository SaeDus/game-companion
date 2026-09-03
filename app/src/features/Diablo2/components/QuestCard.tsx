import type { Quest } from "../types/gameState";

interface QuestCardProps {
  quest: Quest;
}

function QuestCard({ quest }: QuestCardProps) {
  return (
    <article className={`quest-card quest-${quest.State.toLowerCase()}`}>
      <span className="quest-name">{quest.Name}</span>
      <span className="quest-state">{quest.State}</span>
    </article>
  );
}

export default QuestCard;
