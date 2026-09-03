import type { Item } from "../types/gameState";

interface ItemCardProps {
  item: Item;
}

function ItemCard({ item }: ItemCardProps) {
  const displayName = item.Name ?? item.BaseName;

  return (
    <article>
      <h3>{displayName}</h3>

      {item.Name && <p>{item.BaseName}</p>}

      {item.ItemLevel !== undefined && (
        <p>Item Level: {item.ItemLevel}</p>
      )}

      {item.Quality !== undefined && (
        <p>Quality: {item.Quality}</p>
      )}

      {item.Quantity !== undefined && (
        <p>Quantity: {item.Quantity}</p>
      )}

      {item.Defense !== undefined && (
        <p>Defense: {item.Defense}</p>
      )}

      {item.Stats && item.Stats.length > 0 && (
        <ul>
          {item.Stats.map((stat, index) => (
            <li key={index}>{stat}</li>
          ))}
        </ul>
      )}
    </article>
  );
}

export default ItemCard;
