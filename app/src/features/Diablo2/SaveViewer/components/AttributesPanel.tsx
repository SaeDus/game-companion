import type { Character } from "../types/gameState";

interface AttributesPanelProps {
  character: Character;
}

function AttributesPanel({ character }: AttributesPanelProps) {
  return (
    <section>
      <h2>Attributes</h2>

      <ul>
        <li>Strength: {character.Attributes.Strength}</li>
        <li>Dexterity: {character.Attributes.Dexterity}</li>
        <li>Vitality: {character.Attributes.Vitality}</li>
        <li>Energy: {character.Attributes.Energy}</li>
      </ul>

      <p>Unspent Stat Points: {character.UnspentStatPoints}</p>
    </section>
  );
}

export default AttributesPanel;
