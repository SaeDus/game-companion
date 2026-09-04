import type { Character } from "../types/gameState";
import ItemCard from "./ItemCard";

interface InventoryPanelProps {
  character: Character;
}

function InventoryPanel({ character }: InventoryPanelProps) {
  return (
    <section>
      <h2>Inventory</h2>

      {character.Inventory.length === 0 ? (
        <p>Inventory is empty.</p>
      ) : (
        <div className="inventory-grid">
          {character.Inventory.map((item, index) => (
            <ItemCard
              key={item.ItemSeed ?? `$(item.BaseCode}-${index}`}
              item={item}
            />
          ))}
        </div>
      )}
    </section>
  );
}

export default InventoryPanel;
