import hashlib
import json
import sys
from pathlib import Path

SCHEMA_VERSION = 1
SOURCE_FILE = None
SOURCE_TYPE = "embedded"
OUTPUT_FILE = "skilltabs.json"

SKILL_TABS = {
    "0": {
        "class": "Amazon",
        "tree": "Bow and Crossbow Skills",
        "skillPage": 1,
    },
    "1": {
        "class": "Amazon",
        "tree": "Passive and Magic Skills",
        "skillPage": 2,
    },
    "2": {
        "class": "Amazon",
        "tree": "Javelin and Spear Skills",
        "skillPage": 3,
    },
    "8": {
        "class": "Sorceress",
        "tree": "Fire Spells",
        "skillPage": 1,
    },
    "9": {
        "class": "Sorceress",
        "tree": "Lightning Spells",
        "skillPage": 2,
    },
    "10": {
        "class": "Sorceress",
        "tree": "Cold Spells",
        "skillPage": 3,
    },
    "16": {
        "class": "Necromancer",
        "tree": "Curses",
        "skillPage": 1,
    },
    "17": {
        "class": "Necromancer",
        "tree": "Poison and Bone Skills",
        "skillPage": 2,
    },
    "18": {
        "class": "Necromancer",
        "tree": "Summoning Skills",
        "skillPage": 3,
    },
    "24": {
        "class": "Paladin",
        "tree": "Combat Skills",
        "skillPage": 1,
    },
    "25": {
        "class": "Paladin",
        "tree": "Offensive Auras",
        "skillPage": 2,
    },
    "26": {
        "class": "Paladin",
        "tree": "Defensive Auras",
        "skillPage": 3,
    },
    "32": {
        "class": "Barbarian",
        "tree": "Combat Skills",
        "skillPage": 1,
    },
    "33": {
        "class": "Barbarian",
        "tree": "Combat Masteries",
        "skillPage": 2,
    },
    "34": {
        "class": "Barbarian",
        "tree": "Warcries",
        "skillPage": 3,
    },
    "40": {
        "class": "Druid",
        "tree": "Summoning Skills",
        "skillPage": 1,
    },
    "41": {
        "class": "Druid",
        "tree": "Shape Shifting Skills",
        "skillPage": 2,
    },
    "42": {
        "class": "Druid",
        "tree": "Elemental Skills",
        "skillPage": 3,
    },
    "48": {
        "class": "Assassin",
        "tree": "Traps",
        "skillPage": 1,
    },
    "49": {
        "class": "Assassin",
        "tree": "Shadow Disciplines",
        "skillPage": 2,
    },
    "50": {
        "class": "Assassin",
        "tree": "Martial Arts",
        "skillPage": 3,
    },
    "56": {
        "class": "Warlock",
        "tree": "Demon",
        "skillPage": 1,
    },
    "57": {
        "class": "Warlock",
        "tree": "Eldritch",
        "skillPage": 2,
    },
    "58": {
        "class": "Warlock",
        "tree": "Chaos",
        "skillPage": 3,
    },
}


def embedded_source_sha256():
    canonical = json.dumps(
        SKILL_TABS,
        ensure_ascii=False,
        sort_keys=True,
        separators=(",", ":"),
    ).encode("utf-8")

    return hashlib.sha256(canonical).hexdigest()


SOURCE_SHA256 = embedded_source_sha256()


def build_document():
    return {
        "metadata": {
            "schemaVersion": SCHEMA_VERSION,
            "sourceType": SOURCE_TYPE,
            "sourceFile": SOURCE_FILE,
            "sourceSha256": SOURCE_SHA256,
        },
        "data": SKILL_TABS,
    }


def build_skilltabs_json(source_dir: Path, output_dir: Path):
    # source_dir is intentionally unused. This table does not exist as a
    # standalone TXT file in the canonical D2R-Excel source set.
    _ = Path(source_dir)
    output_dir = Path(output_dir)

    try:
        output_dir.mkdir(parents=True, exist_ok=True)
        output_path = output_dir / OUTPUT_FILE
        output_path.write_text(
            json.dumps(build_document(), indent=4, ensure_ascii=False) + "\n",
            encoding="utf-8",
        )
    except OSError:
        return False, "skilltabs.json could not be written."

    return True, str(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        raise SystemExit("Usage: build_skilltabs_json.py <source_dir> <output_dir>")

    success, message = build_skilltabs_json(Path(sys.argv[1]), Path(sys.argv[2]))

    if not success:
        raise SystemExit(message)

    print(message)
