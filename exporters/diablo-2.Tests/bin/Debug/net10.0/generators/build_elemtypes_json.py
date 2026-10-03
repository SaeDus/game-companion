import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "elemtypes.txt"
OUTPUT_FILE = "elemtypes.json"


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


def as_text(value):
    value = (value or "").strip()
    return value if value else None


def build_elemtypes_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "elemtypes.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {"Elemental Type", "Code"}

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "elemtypes.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "elemtypes.txt is invalid."

        result = {}

        for index, row in enumerate(reader):
            name = as_text(row.get("Elemental Type"))

            if name is None:
                return False, "elemtypes.txt is invalid."

            result[str(index)] = {
                "name": name,
            }

    if not result:
        return False, "elemtypes.txt is invalid."

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_elemtypes_json.py <source_dir> <output_dir>")

    success, message = build_elemtypes_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
