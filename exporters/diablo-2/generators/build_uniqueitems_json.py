import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 2
SOURCE_FILE = "uniqueitems.txt"
OUTPUT_FILE = "uniqueitems.json"


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

    for i in range(1, 13):
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


def build_uniqueitems_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "uniqueitems.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "*ID", "index", "code", "lvl", "lvl req", "rarity", "spawnable", "carry1",
    }

    for i in range(1, 13):
        expected_columns.update({f"prop{i}", f"par{i}", f"min{i}", f"max{i}"})

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "uniqueitems.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "uniqueitems.txt is invalid."

        result = {}

        for row in reader:
            unique_id = as_int(row.get("*ID"))
            name = as_text(row.get("index"))
            base_code = as_text(row.get("code"))

            # The canonical D2R source contains named/numbered placeholder
            # rows whose base-item code is intentionally blank. They are valid
            # source rows but cannot produce usable unique-item metadata.
            if unique_id is None or not name or not base_code:
                continue

            result[str(unique_id)] = {
                "name": name,
                "baseCode": base_code,
                "level": as_int(row.get("lvl")),
                "levelRequirement": as_int(row.get("lvl req")),
                "rarity": as_int(row.get("rarity")),
                "spawnable": as_bool(row.get("spawnable")),
                "carryOne": as_bool(row.get("carry1")),
                "properties": build_properties(row),
            }

    result = dict(sorted(result.items(), key=lambda pair: int(pair[0])))

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_uniqueitems_json.py <source_dir> <output_dir>")

    success, message = build_uniqueitems_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
