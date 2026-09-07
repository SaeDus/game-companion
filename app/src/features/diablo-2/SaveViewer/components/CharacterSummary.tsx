import type { Character } from "../types/gameState";

interface CharacterSummaryProps {
  character: Character;
}

function CharacterSummary({ character }: CharacterSummaryProps) {
  return (
    <section>
      <h1>{character.Name}</h1>

      <p>
        Level {character.Level} {character.Class}
      </p>
    </section>
  );
}

export default CharacterSummary;
