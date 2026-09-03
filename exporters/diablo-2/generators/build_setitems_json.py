import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "setitems.txt"
OUTPUT_FILE = "setitems.json"


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


def build_properties(row):
    properties = []

    for i in range(1, 10):
        code = as_text(row.get(f"prop{i}"))

        if not code:
            continue

        properties.append({
            "code": code,
            "parameter": as_text(row.get(f"par{i}")),
            "min": as_int(row.get(f"min{i}")),
            "max": as_int(row.get(f"max{i}")),
        })

    return properties


def build_set_bonuses(row):
    bonuses = []

    for tier in range(1, 6):
        tier_properties = []

        for suffix in ("a", "b"):
            code = as_text(row.get(f"aprop{tier}{suffix}"))

            if not code:
                continue

            tier_properties.append({
                "code": code,
                "parameter": as_text(row.get(f"apar{tier}{suffix}")),
                "min": as_int(row.get(f"amin{tier}{suffix}")),
                "max": as_int(row.get(f"amax{tier}{suffix}")),
            })

        if tier_properties:
            bonuses.append({
                "tier": tier,
                "properties": tier_properties,
            })

    return bonuses


def build_setitems_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "setitems.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "*ID", "index", "set", "item", "lvl", "lvl req", "rarity", "spawnable",
    }

    for i in range(1, 10):
        expected_columns.update({f"prop{i}", f"par{i}", f"min{i}", f"max{i}"})

    for tier in range(1, 6):
        for suffix in ("a", "b"):
            expected_columns.update(
                {
                    f"aprop{tier}{suffix}",
                    f"apar{tier}{suffix}",
                    f"amin{tier}{suffix}",
                    f"amax{tier}{suffix}",
                }
            )

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "setitems.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "setitems.txt is invalid."

        result = {}

        for row in reader:
            set_item_id = as_int(row.get("*ID"))
            name = as_text(row.get("index"))
            set_name = as_text(row.get("set"))
            base_code = as_text(row.get("item"))

            if set_item_id is None or not name:
                continue

            if not set_name or not base_code:
                return False, "setitems.txt is invalid."

            result[str(set_item_id)] = {
                "name": name,
                "set": set_name,
                "baseCode": base_code,
                "level": as_int(row.get("lvl")),
                "levelRequirement": as_int(row.get("lvl req")),
                "rarity": as_int(row.get("rarity")),
                "spawnable": as_bool(row.get("spawnable")),
                "properties": build_properties(row),
                "setBonuses": build_set_bonuses(row),
            }

    result = dict(sorted(result.items(), key=lambda pair: int(pair[0])))

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_setitems_json.py <source_dir> <output_dir>")

    success, message = build_setitems_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
