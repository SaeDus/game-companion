import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "weapons.txt"
OUTPUT_FILE = "weapons.json"


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

def normalize_upgrade(value):
    value = as_text(value)
    return None if value in (None, "xxx") else value


def build_weapons_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "weapons.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "code", "name", "level", "levelreq", "rarity", "spawnable",
        "type", "type2", "mindam", "maxdam", "2handed", "2handmindam",
        "2handmaxdam", "minmisdam", "maxmisdam", "rangeadder", "speed",
        "reqstr", "reqdex", "durability", "nodurability", "magic lvl",
        "auto prefix", "gemsockets", "normcode", "ubercode", "ultracode",
        "invwidth", "invheight", "cost", "gamble cost", "wclass",
        "2handedwclass", "stackable", "minstack", "maxstack", "spawnstack",
        "NightmareUpgrade", "HellUpgrade", "ShowLevel", "Nameable",
    }

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "weapons.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "weapons.txt is invalid."

        result = {}

        for row in reader:
            code = as_text(row.get("code"))
            name = as_text(row.get("name"))

            # Skip separator / non-item rows.
            if not code or not name:
                continue

            result[code] = {
                "name": name,
                "level": as_int(row.get("level")),
                "levelRequirement": as_int(row.get("levelreq")),
                "rarity": as_int(row.get("rarity")),
                "spawnable": as_bool(row.get("spawnable")),
                "type": as_text(row.get("type")),
                "type2": as_text(row.get("type2")),
                "minDamage": as_int(row.get("mindam")),
                "maxDamage": as_int(row.get("maxdam")),
                "twoHanded": as_bool(row.get("2handed")),
                "twoHandMinDamage": as_int(row.get("2handmindam")),
                "twoHandMaxDamage": as_int(row.get("2handmaxdam")),
                "minMissileDamage": as_int(row.get("minmisdam")),
                "maxMissileDamage": as_int(row.get("maxmisdam")),
                "rangeAdder": as_int(row.get("rangeadder")),
                "speed": as_int(row.get("speed")),
                "requiredStrength": as_int(row.get("reqstr")),
                "requiredDexterity": as_int(row.get("reqdex")),
                "durability": as_int(row.get("durability")),
                "noDurability": as_bool(row.get("nodurability")),
                "magicLevel": as_int(row.get("magic lvl")),
                "autoPrefix": as_int(row.get("auto prefix")),
                "maxSockets": as_int(row.get("gemsockets")),
                "normalCode": as_text(row.get("normcode")),
                "exceptionalCode": as_text(row.get("ubercode")),
                "eliteCode": as_text(row.get("ultracode")),
                "inventoryWidth": as_int(row.get("invwidth")),
                "inventoryHeight": as_int(row.get("invheight")),
                "baseCost": as_int(row.get("cost")),
                "gambleCost": as_int(row.get("gamble cost")),
                "weaponClass": as_text(row.get("wclass")),
                "twoHandedWeaponClass": as_text(row.get("2handedwclass")),
                "stackable": as_bool(row.get("stackable")),
                "minStack": as_int(row.get("minstack")),
                "maxStack": as_int(row.get("maxstack")),
                "spawnStack": as_int(row.get("spawnstack")),
                "nightmareUpgrade": normalize_upgrade(row.get("NightmareUpgrade")),
                "hellUpgrade": normalize_upgrade(row.get("HellUpgrade")),
                "showLevel": as_bool(row.get("ShowLevel")),
                "nameable": as_bool(row.get("Nameable")),
            }

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_weapons_json.py <source_dir> <output_dir>")

    success, message = build_weapons_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
