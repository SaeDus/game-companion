import type { Item } from "../types/gameState";
import ItemCard from "./ItemCard";

interface StashPanelProps {
  stash: Item[];
}

function StashPanel({ stash }: StashPanelProps) {
  return (
    <section>
      <h2>Stash</h2>

      {stash.length === 0 ? (
        <p>Stash is empty.</p>
      ) : (
        <div className="inventory-grid">
          {stash.map((item, index) => (
            <ItemCard
              key={item.ItemSeed ?? `${item.BaseCode}-${index}`}
              item={item}
            />
          ))}
        </div>
      )}
    </section>
  );
}

export default StashPanel;
