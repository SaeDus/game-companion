import csv
import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 2
SOURCE_FILE = "hireling.txt"
OUTPUT_FILE = "hirelings.json"


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


def compact_dict(value):
    """
    Recursively remove dictionary keys whose values are None.
    Empty lists/dicts are preserved when structurally meaningful.
    """

    if isinstance(value, dict):
        return {
            key: compact_dict(item) for key, item in value.items() if item is not None
        }

    if isinstance(value, list):
        return [compact_dict(item) for item in value]

    return value


def build_skills(row):
    skills = []

    for i in range(1, 7):
        name = as_text(row.get(f"Skill{i}"))

        if not name:
            continue

        skills.append(
            compact_dict(
                {
                    "name": name,
                    "mode": as_int(row.get(f"Mode{i}")),
                    "chance": as_int(row.get(f"Chance{i}")),
                    "chancePerLevel": as_int(row.get(f"ChancePerLvl{i}")),
                    "level": as_int(row.get(f"Level{i}")),
                    "levelPerLevel": as_int(row.get(f"LvlPerLvl{i}")),
                }
            )
        )

    return skills


def build_identity(row):
    return compact_dict(
        {
            "class": as_int(row.get("Class")),
            "act": as_int(row.get("Act")),
            "difficulty": as_int(row.get("Difficulty")),
            "hireling": as_text(row.get("Hireling")),
            "subType": (as_text(row.get("*SubType")) or as_text(row.get("SubType"))),
            "nameFirst": as_int(row.get("NameFirst")),
            "nameLast": as_int(row.get("NameLast")),
        }
    )


def build_progression_row(row):
    """
    Preserve one source row from hireling.txt.

    Multiple hireling.txt rows can share the same Id.
    The Level column is therefore treated as a progression
    anchor row, not as a unique hireling.
    """

    return compact_dict(
        {
            "version": as_int(row.get("Version")),
            "progression": {
                "level": as_int(row.get("Level")),
                "experiencePerLevel": as_int(row.get("Exp/Lvl")),
            },
            "stats": {
                "hitPoints": {
                    "base": as_int(row.get("HP")),
                    "perLevel": as_int(row.get("HP/Lvl")),
                },
                "defense": {
                    "base": as_int(row.get("Defense")),
                    "perLevel": as_int(row.get("Def/Lvl")),
                },
                "strength": {
                    "base": as_int(row.get("Str")),
                    "perLevel": as_int(row.get("Str/Lvl")),
                },
                "dexterity": {
                    "base": as_int(row.get("Dex")),
                    "perLevel": as_int(row.get("Dex/Lvl")),
                },
                "attackRating": {
                    "base": as_int(row.get("AR")),
                    "perLevel": as_int(row.get("AR/Lvl")),
                },
                "damage": {
                    "min": as_int(row.get("Dmg-Min")),
                    "max": as_int(row.get("Dmg-Max")),
                    "perLevel": as_int(row.get("Dmg/Lvl")),
                },
            },
            "resistances": {
                "fire": {
                    "base": as_int(row.get("ResistFire")),
                    "perLevel": as_int(row.get("ResistFire/Lvl")),
                },
                "cold": {
                    "base": as_int(row.get("ResistCold")),
                    "perLevel": as_int(row.get("ResistCold/Lvl")),
                },
                "lightning": {
                    "base": as_int(row.get("ResistLightning")),
                    "perLevel": as_int(row.get("ResistLightning/Lvl")),
                },
                "poison": {
                    "base": as_int(row.get("ResistPoison")),
                    "perLevel": as_int(row.get("ResistPoison/Lvl")),
                },
            },
            "skills": build_skills(row),
        }
    )


def build_hirelings_json(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return False, f"Source directory does not exist: {source_dir}"

    source_path = source_dir / SOURCE_FILE

    if not source_path.is_file():
        return False, "hireling.txt was not found."

    output_dir.mkdir(parents=True, exist_ok=True)
    output_path = output_dir / OUTPUT_FILE

    expected_columns = {
        "Id", "Class", "Act", "Difficulty", "Hireling", "NameFirst", "NameLast",
        "Version", "Level", "Exp/Lvl", "HP", "HP/Lvl", "Defense", "Def/Lvl",
        "Str", "Str/Lvl", "Dex", "Dex/Lvl", "AR", "AR/Lvl", "Dmg-Min",
        "Dmg-Max", "Dmg/Lvl", "ResistFire", "ResistFire/Lvl", "ResistCold",
        "ResistCold/Lvl", "ResistLightning", "ResistLightning/Lvl", "ResistPoison",
        "ResistPoison/Lvl",
    }

    for i in range(1, 7):
        expected_columns.update(
            {
                f"Skill{i}", f"Mode{i}", f"Chance{i}", f"ChancePerLvl{i}",
                f"Level{i}", f"LvlPerLvl{i}",
            }
        )

    with source_path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter="\t")

        if reader.fieldnames is None:
            return False, "hireling.txt is invalid."

        if "*SubType" not in reader.fieldnames and "SubType" not in reader.fieldnames:
            return False, "hireling.txt is invalid."

        if any(column not in reader.fieldnames for column in expected_columns):
            return False, "hireling.txt is invalid."

        # hireling.txt legitimately reuses numeric Id values across source
        # versions. In the canonical D2R data, Version 0 and Version 100 can
        # describe different identities for the same Id. Validate/group each
        # version independently, then expose the highest version for each Id so
        # consumers can keep using a simple numeric-id lookup.
        versioned_result = {}

        for row in reader:
            hireling_id = as_int(row.get("Id"))

            if hireling_id is None:
                if any((value or "").strip() for value in row.values()):
                    return False, "hireling.txt is invalid."
                continue

            version = as_int(row.get("Version"))

            if version is None:
                return False, "hireling.txt is invalid."

            group_key = (hireling_id, version)
            identity = build_identity(row)
            progression_row = build_progression_row(row)

            if group_key not in versioned_result:
                versioned_result[group_key] = {
                    "id": hireling_id,
                    "identity": identity,
                    "rows": [],
                }
            elif versioned_result[group_key]["identity"] != identity:
                return False, "hireling.txt is invalid."

            versioned_result[group_key]["rows"].append(progression_row)

    result = {}
    selected_versions = {}

    for (hireling_id, version), entry in versioned_result.items():
        key = str(hireling_id)

        if key not in result or version > selected_versions[key]:
            result[key] = entry
            selected_versions[key] = version

    result = dict(sorted(result.items(), key=lambda pair: int(pair[0])))

    for entry in result.values():
        entry["rows"].sort(
            key=lambda item: item.get("progression", {}).get("level", -1)
        )

    output_path.write_text(
        json.dumps(build_document(result, source_path), indent=4, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_hirelings_json.py <source_dir> <output_dir>")

    success, message = build_hirelings_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
