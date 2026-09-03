import type { Character, Item } from "../types/gameState";
import ItemCard from "./ItemCard";

interface EquipmentPanelProps {
  character: Character;
}

const equipmentSlots = [
  "RightArm",
  "LeftArm",
  "Head",
  "Torso",
  "Gloves",
  "Belt",
  "Feet",
  "Neck",
  "RightRing",
  "LeftRing",
] as const;

const slotLabels: Record<string, string> = {
  Head: "Helm",
  Neck: "Amulet",
  Torso: "Armor",
  RightArm: "Weapon",
  LeftArm: "Offhand",
  Gloves: "Gloves",
  Belt: "Belt",
  Feet: "Boots",
  LeftRing: "Ring",
  RightRing: "Ring",
}

function EquipmentPanel({ character }: EquipmentPanelProps) {
  const equipmentBySlot = new Map<string, Item>();

  for (const item of character.Equipment) {
    if (item.Slot) {
      equipmentBySlot.set(item.Slot, item);
    }
  }

  return (
    <section>
      <h2>Equipment</h2>

      <div className="equipment-layout">
        {equipmentSlots.map((slot) => {
          const item = equipmentBySlot.get(slot);

          return (
            <div key={slot} className={`equipment-slot slot-${slot}`}>
              <span className="equipment-slot-label">
                {slotLabels[slot] ?? slot}
              </span>

              {item ? (
                <ItemCard
                  key={item.ItemSeed ?? `${item.BaseCode}-${slot}`}
                  item={item}
                />
              ) : (
                <div className="equipment-slot-empty">
                  Empty
                </div>
              )}
            </div>
          );
        })}
      </div>
    </section>
  );
}

export default EquipmentPanel;
