import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "misc.txt"
OUTPUT_FILE = "misc.json"


def source_sha256(source_path: Path):
    digest = hashlib.sha256()

    with source_path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            digest.update(chunk)

    return digest.hexdigest()


def build_document(data, source_path: Path):
    return {
        "metadata": {
            "schemaVersion": SCHEMA_VERSION,
            "sourceFile": SOURCE_FILE,
            "sourceSha256": source_sha256(source_path),
        },
        "data": data,
    }


# Broad misc item categories where every matching item can matter to
# equipment, socketing, crafting, stash, or build decisions.
ALLOWED_TYPES = {
    "ring",  # Rings
    "amul",  # Amulets
    "scha",  # Small Charms
    "mcha",  # Large Charms
    "lcha",  # Grand Charms
    "jewl",  # Jewels
    "rune",  # Runes
    "gema",  # Amethyst
    "gemd",  # Diamond
    "geme",  # Emerald
    "gemr",  # Ruby
    "gems",  # Sapphire
    "gemt",  # Topaz
    "gemz",  # Skull
}

# Strategic progression/endgame items that live under generic misc/quest types.
# These are explicitly whitelisted so we do NOT have to import every quest item.
STRATEGIC_CODES = {
    # Pandemonium keys
    "pk1",  # Key of Terror
    "pk2",  # Key of Hate
    "pk3",  # Key of Destruction
    # Uber organs
    "dhn",  # Diablo's Horn
    "bey",  # Baal's Eye
    "mbr",  # Mephisto's Brain
    # Essences + respec token
    "tes",  # Twisted Essence of Suffering
    "ceh",  # Charged Essence of Hatred
    "bet",  # Burning Essence of Terror
    "fed",  # Festering Essence of Destruction
    "toa",  # Token of Absolution
    # Endgame trophy/reward
    "std",  # Standard of Heroes
    # Worldstone Shards
    "xa1",  # Western Worldstone Shard
    "xa2",  # Eastern Worldstone Shard
    "xa3",  # Southern Worldstone Shard
    "xa4",  # Deep Worldstone Shard
    "xa5",  # Northern Worldstone Shard
    # Reign of the Warlock Uber Ancient summon materials
    "ua1",
    "ua2",
    "ua3",
    "ua4",
    "ua5",
    # Reign of the Warlock Uber Ancient upgrade materials
    "um1",
    "um2",
    "um3",
    "um4",
    "um5",
    "um6",
    # Special crafting material
    "cjw",  # Colossal Jewel
}


def as_int(value):
    value = (value or "").strip()
    if value == "":
        return None
    try:
        return int(value)
    except ValueError:
        return None


def as_bool(value):
    iv = as_int(value)
    return None if iv is None else bool(iv)


def as_text(value):
    value = (value or "").strip()
    return value if value else None


def build_misc_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "misc.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "code", "name", "type", "type2", "level", "levelreq", "rarity",
        "spawnable", "invwidth", "invheight", "unique", "quest",
        "questdiffcheck", "cost", "gamble cost", "reqstr", "reqdex",
        "stackable", "minstack", "maxstack", "spawnstack", "gemsockets",
        "gemapplytype", "Nameable", "AdvancedStashStackable",
    }

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "misc.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "misc.txt is invalid."

        result = {}

        for row in reader:
            code = as_text(row.get("code"))
            name = as_text(row.get("name"))
            item_type = as_text(row.get("type"))

            if not code or not name:
                continue

            include_by_type = item_type in ALLOWED_TYPES
            include_by_code = code in STRATEGIC_CODES

            if not include_by_type and not include_by_code:
                continue

            if include_by_type and include_by_code:
                strategic_category = "gear+progression"
            elif include_by_code:
                strategic_category = "progression"
            else:
                strategic_category = "gear"

            result[code] = {
                "name": name,
                "type": item_type,
                "type2": as_text(row.get("type2")),
                "level": as_int(row.get("level")),
                "levelRequirement": as_int(row.get("levelreq")),
                "rarity": as_int(row.get("rarity")),
                "spawnable": as_bool(row.get("spawnable")),
                "inventoryWidth": as_int(row.get("invwidth")),
                "inventoryHeight": as_int(row.get("invheight")),
                "unique": as_bool(row.get("unique")),
                "quest": as_int(row.get("quest")),
                "questDifficultyCheck": as_bool(row.get("questdiffcheck")),
                "baseCost": as_int(row.get("cost")),
                "gambleCost": as_int(row.get("gamble cost")),
                "requiredStrength": as_int(row.get("reqstr")),
                "requiredDexterity": as_int(row.get("reqdex")),
                "stackable": as_bool(row.get("stackable")),
                "minStack": as_int(row.get("minstack")),
                "maxStack": as_int(row.get("maxstack")),
                "spawnStack": as_int(row.get("spawnstack")),
                "maxSockets": as_int(row.get("gemsockets")),
                "gemApplyType": as_int(row.get("gemapplytype")),
                "nameable": as_bool(row.get("Nameable")),
                "advancedStashStackable": as_bool(row.get("AdvancedStashStackable")),
                "strategicCategory": strategic_category,
            }

    result = dict(sorted(result.items()))

    if any(code not in result for code in STRATEGIC_CODES):
        return False, "misc.txt is invalid."

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_misc_json.py <source_dir> <output_dir>")

    success, message = build_misc_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
