import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "sets.txt"
OUTPUT_FILE = "sets.json"


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


def as_text(value):
    value = (value or "").strip()
    return value if value else None


def build_property(row, code_key, param_key, min_key, max_key):
    code = as_text(row.get(code_key))

    if not code:
        return None

    return {
        "code": code,
        "parameter": as_text(row.get(param_key)),
        "min": as_int(row.get(min_key)),
        "max": as_int(row.get(max_key)),
    }


def build_partial_bonuses(row):
    partial_bonuses = []

    for pieces_required in range(2, 6):
        properties = []

        for suffix in ("a", "b"):
            prop = build_property(
                row,
                f"PCode{pieces_required}{suffix}",
                f"PParam{pieces_required}{suffix}",
                f"PMin{pieces_required}{suffix}",
                f"PMax{pieces_required}{suffix}",
            )

            if prop is not None:
                properties.append(prop)

        if properties:
            partial_bonuses.append({
                "piecesRequired": pieces_required,
                "properties": properties,
            })

    return partial_bonuses


def build_full_set_bonuses(row):
    full_set_bonuses = []

    for i in range(1, 9):
        prop = build_property(
            row,
            f"FCode{i}",
            f"FParam{i}",
            f"FMin{i}",
            f"FMax{i}",
        )

        if prop is not None:
            full_set_bonuses.append(prop)

    return full_set_bonuses


def build_sets_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "sets.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = set()

    for pieces_required in range(2, 6):
        for suffix in ("a", "b"):
            expected_columns.update(
                {
                    f"PCode{pieces_required}{suffix}",
                    f"PParam{pieces_required}{suffix}",
                    f"PMin{pieces_required}{suffix}",
                    f"PMax{pieces_required}{suffix}",
                }
            )

    for i in range(1, 9):
        expected_columns.update({f"FCode{i}", f"FParam{i}", f"FMin{i}", f"FMax{i}"})

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "sets.txt is invalid."

        if "index" not in reader.fieldnames and "name" not in reader.fieldnames:
            return False, "sets.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "sets.txt is invalid."

        result = {}

        for row in reader:
            set_name = as_text(row.get("index")) or as_text(row.get("name"))

            if not set_name:
                continue

            result[set_name] = {
                "partialBonuses": build_partial_bonuses(row),
                "fullSetBonuses": build_full_set_bonuses(row),
            }

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_sets_json.py <source_dir> <output_dir>")

    success, message = build_sets_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
