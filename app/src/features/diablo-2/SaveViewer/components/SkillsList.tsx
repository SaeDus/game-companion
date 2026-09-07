import type { Character } from "../types/gameState";

interface SkillsListProps {
  character: Character;
}

function SkillsList({ character }: SkillsListProps) {
  return (
    <section>
      <h2>Skills</h2>

      <ul>
        {character.Skills.map((skill) => (
          <li key={skill.Id}>
            {skill.Name}: {skill.Level}
          </li>
        ))}
      </ul>

      <p>Unspent Skill Points: {character.UnspentSkillPoints}</p>
    </section>
  );
}

export default SkillsList;
