import type { Mercenary } from "../types/gameState";

interface MercenarySummaryProps {
  mercenary: Mercenary;
}

function MercenarySummary({ mercenary }: MercenarySummaryProps) {
  return (
    <section>
      <h3>Mercenary</h3>

      <h4>Level {mercenary.Level} {mercenary.Hireling} - {mercenary.SubType}</h4>

      <p>Attributes:</p>

      <ul>
        <li>Strength: {mercenary.Strength}</li>
        <li>Dexterity: {mercenary.Dexterity}</li>
      </ul>

      <p>Skills:</p>

      <ul>
        {mercenary.Skills.map((item) => (
          <li>{item}</li>
        ))}
      </ul>
    </section>
  );
}

export default MercenarySummary;
