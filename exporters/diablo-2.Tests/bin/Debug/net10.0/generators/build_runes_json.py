import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "runes.txt"
OUTPUT_FILE = "runes.json"


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


def build_properties(row):
    properties = []

    for i in range(1, 8):
        code = as_text(row.get(f"T1Code{i}"))

        if not code:
            continue

        properties.append(
            {
                "code": code,
                "parameter": as_text(row.get(f"T1Param{i}")),
                "min": as_int(row.get(f"T1Min{i}")),
                "max": as_int(row.get(f"T1Max{i}")),
            }
        )

    return properties


def build_runes_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "runes.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "Name", "*Rune Name", "complete", "disallowCraftingInNonLadder",
    }

    for i in range(1, 7):
        expected_columns.update({f"itype{i}", f"Rune{i}"})

    for i in range(1, 4):
        expected_columns.add(f"etype{i}")

    for i in range(1, 8):
        expected_columns.update(
            {f"T1Code{i}", f"T1Param{i}", f"T1Min{i}", f"T1Max{i}"}
        )

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "runes.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "runes.txt is invalid."

        result = {}

        for row in reader:
            internal_name = as_text(row.get("Name"))
            display_name = as_text(row.get("*Rune Name"))

            if not internal_name or not display_name:
                continue

            allowed_item_types = nonempty_list(
                *(as_text(row.get(f"itype{i}")) for i in range(1, 7))
            )

            excluded_item_types = nonempty_list(
                *(as_text(row.get(f"etype{i}")) for i in range(1, 4))
            )

            runes = nonempty_list(
                *(as_text(row.get(f"Rune{i}")) for i in range(1, 7))
            )

            result[internal_name] = {
                "name": display_name,
                "complete": as_bool(row.get("complete")),
                "disallowInNonLadder": as_bool(
                    row.get("disallowCraftingInNonLadder")
                ),
                "allowedItemTypes": allowed_item_types,
                "excludedItemTypes": excluded_item_types,
                "runes": runes,
                "properties": build_properties(row),
            }

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_runes_json.py <source_dir> <output_dir>")

    success, message = build_runes_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
