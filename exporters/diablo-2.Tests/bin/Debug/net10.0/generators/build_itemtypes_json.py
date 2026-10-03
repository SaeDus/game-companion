import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "itemtypes.txt"
OUTPUT_FILE = "itemtypes.json"


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

def nonempty_list(*values):
    return [value for value in values if value not in (None, "")]

def build_socket_caps(row):
    max1 = as_int(row.get("MaxSockets1"))
    threshold1 = as_int(row.get("MaxSocketsLevelThreshold1"))

    max2 = as_int(row.get("MaxSockets2"))
    threshold2 = as_int(row.get("MaxSocketsLevelThreshold2"))

    max3 = as_int(row.get("MaxSockets3"))

    caps = []

    if max1 is not None:
        entry = {"maxSockets": max1}
        if threshold1 is not None:
            entry["maxItemLevel"] = threshold1
        caps.append(entry)

    if max2 is not None:
        entry = {"maxSockets": max2}
        if threshold2 is not None:
            entry["maxItemLevel"] = threshold2
        caps.append(entry)

    if max3 is not None:
        caps.append({"maxSockets": max3})

    return caps


def build_itemtypes_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "itemtypes.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "Code", "ItemType", "Equiv1", "Equiv2", "Body", "BodyLoc1",
        "BodyLoc2", "Shoots", "Quiver", "Throwable", "AutoStack", "Magic",
        "Rare", "Normal", "Beltable", "MaxSockets1",
        "MaxSocketsLevelThreshold1", "MaxSockets2",
        "MaxSocketsLevelThreshold2", "MaxSockets3", "TreasureClass", "Rarity",
        "StaffMods", "Class", "UICategory", "RunewordCategory1",
        "RunewordCategory2", "Restricted",
    }

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "itemtypes.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "itemtypes.txt is invalid."

        result = {}

        for row in reader:
            code = as_text(row.get("Code"))
            name = as_text(row.get("ItemType"))

            # Skip separator / malformed rows already present in the source table.
            if not code or not name:
                continue

            body_locations = nonempty_list(
                as_text(row.get("BodyLoc1")),
                as_text(row.get("BodyLoc2")),
            )

            runeword_categories = nonempty_list(
                as_text(row.get("RunewordCategory1")),
                as_text(row.get("RunewordCategory2")),
            )

            result[code] = {
                "name": name,
                "equivalentType1": as_text(row.get("Equiv1")),
                "equivalentType2": as_text(row.get("Equiv2")),
                "body": as_bool(row.get("Body")),
                "bodyLocations": body_locations,
                "shoots": as_text(row.get("Shoots")),
                "quiver": as_text(row.get("Quiver")),
                "throwable": as_bool(row.get("Throwable")),
                "autoStack": as_bool(row.get("AutoStack")),
                "canBeMagic": as_bool(row.get("Magic")),
                "canBeRare": as_bool(row.get("Rare")),
                "canBeNormal": as_bool(row.get("Normal")),
                "beltable": as_bool(row.get("Beltable")),
                "socketCaps": build_socket_caps(row),
                "treasureClass": as_int(row.get("TreasureClass")),
                "rarity": as_int(row.get("Rarity")),
                "staffMods": as_text(row.get("StaffMods")),
                "class": as_text(row.get("Class")),
                "uiCategory": as_text(row.get("UICategory")),
                "runewordCategories": runeword_categories,
                "restricted": as_text(row.get("Restricted")),
            }

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_itemtypes_json.py <source_dir> <output_dir>")

    success, message = build_itemtypes_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
