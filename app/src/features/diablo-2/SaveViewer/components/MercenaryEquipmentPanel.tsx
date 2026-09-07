import type { Mercenary, Item } from "../types/gameState";
import ItemCard from "./ItemCard";

interface MercenaryEquipmentPanelProps {
  mercenary: Mercenary;
}

const equipmentSlots = [
  "Head",
  "Torso",
  "RightArm",
  "LeftArm",
] as const;

const slotLabels: Record<string, string> = {
  Head: "Helm",
  Torso: "Armor",
  RightArm: "Weapon",
  LeftArm: "Offhand",
};

function MercenaryEquipmentPanel({ mercenary }: MercenaryEquipmentPanelProps) {
  const equipmentBySlot = new Map<string, Item>();

  for (const item of mercenary.Equipment) {
    if (item.Slot) {
      equipmentBySlot.set(item.Slot, item);
    }
  }

  return (
    <section>
      <h2>Equipment</h2>

      <div className="mercenary-equipment-layout">
        {equipmentSlots.map((slot) => {
          const item = equipmentBySlot.get(slot);

          return (
            <div
              key={slot}
              className={`equipment-slot slot-${slot}`}
            >
              <span className="equipment-slot-label">
                {slotLabels[slot] ?? slot}
              </span>

              {item ? (
                <ItemCard item={item} />
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

export default MercenaryEquipmentPanel;
