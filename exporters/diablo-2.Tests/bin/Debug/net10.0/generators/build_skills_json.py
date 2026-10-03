import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = "skills.txt"
OUTPUT_FILE = "skills.json"


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


CLASS_NAMES = {
    "ama": "Amazon",
    "sor": "Sorceress",
    "nec": "Necromancer",
    "pal": "Paladin",
    "bar": "Barbarian",
    "dru": "Druid",
    "ass": "Assassin",
    "war": "Warlock",
}

def as_int(value):
    value = (value or "").strip()
    if not value:
        return None
    try:
        return int(value)
    except ValueError:
        return None

def as_text(value):
    value = (value or "").strip()
    return value if value else None


def build_skills_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "skills.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "skill", "*Id", "charclass", "reqlevel", "reqskill1", "reqskill2", "reqskill3",
    }

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "skills.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "skills.txt is invalid."

        rows = list(reader)

    # First pass: map prerequisite skill names to numeric *Id values.
    skill_name_to_id = {}

    for row in rows:
        name = as_text(row.get("skill"))
        skill_id = as_int(row.get("*Id"))

        if name is not None and skill_id is not None:
            skill_name_to_id[name] = skill_id

    result = {}

    for row in rows:
        class_code = as_text(row.get("charclass"))

        # Only export playable-class skills.
        if class_code not in CLASS_NAMES:
            continue

        skill_id = as_int(row.get("*Id"))
        name = as_text(row.get("skill"))

        if skill_id is None or name is None:
            continue

        prerequisites = []

        for column in ("reqskill1", "reqskill2", "reqskill3"):
            raw_requirement = as_text(row.get(column))

            if raw_requirement is None:
                continue

            # Usually these fields contain the skill's internal/name string,
            # but accept a numeric ID too in case the source table changes.
            requirement_id = as_int(raw_requirement)

            if requirement_id is None:
                requirement_id = skill_name_to_id.get(raw_requirement)

            if requirement_id is None:
                return False, "skills.txt is invalid."

            prerequisites.append(requirement_id)

        result[str(skill_id)] = {
            "name": name,
            "class": CLASS_NAMES[class_code],
            "requiredLevel": as_int(row.get("reqlevel")),
            "prerequisites": prerequisites,
        }

    result = dict(sorted(result.items(), key=lambda pair: int(pair[0])))

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_skills_json.py <source_dir> <output_dir>")

    success, message = build_skills_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
