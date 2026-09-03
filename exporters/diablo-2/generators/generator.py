import hashlib
import json
import sys
import traceback
from pathlib import Path

import build_armor_json as armor_builder
import build_elemtypes_json as elemtypes_builder
import build_hirelings_json as hirelings_builder
import build_itemtypes_json as itemtypes_builder
import build_misc_json as misc_builder
import build_playerclass_json as playerclass_builder
import build_runes_json as runes_builder
import build_setitems_json as setitems_builder
import build_sets_json as sets_builder
import build_skills_json as skills_builder
import build_skilltabs_json as skilltabs_builder
import build_uniqueitems_json as uniqueitems_builder
import build_weapons_json as weapons_builder

BUILDERS = (
    ("armor", armor_builder, armor_builder.build_armor_json),
    ("elemtypes", elemtypes_builder, elemtypes_builder.build_elemtypes_json),
    ("hirelings", hirelings_builder, hirelings_builder.build_hirelings_json),
    ("itemtypes", itemtypes_builder, itemtypes_builder.build_itemtypes_json),
    ("misc", misc_builder, misc_builder.build_misc_json),
    ("playerclass", playerclass_builder, playerclass_builder.build_playerclass_json),
    ("runes", runes_builder, runes_builder.build_runes_json),
    ("setitems", setitems_builder, setitems_builder.build_setitems_json),
    ("sets", sets_builder, sets_builder.build_sets_json),
    ("skills", skills_builder, skills_builder.build_skills_json),
    ("skilltabs", skilltabs_builder, skilltabs_builder.build_skilltabs_json),
    (
        "uniqueitems",
        uniqueitems_builder,
        uniqueitems_builder.build_uniqueitems_json,
    ),
    ("weapons", weapons_builder, weapons_builder.build_weapons_json),
)


def sha256_file(path: Path):
    digest = hashlib.sha256()

    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            digest.update(chunk)

    return digest.hexdigest()


def output_is_current(
    output_path: Path,
    source_path: Path | None,
    source_name: str | None,
    schema_version: int,
    embedded_source_hash: str | None = None,
):
    if not output_path.is_file():
        return False

    try:
        document = json.loads(output_path.read_text(encoding="utf-8"))
    except (OSError, UnicodeError, json.JSONDecodeError):
        return False

    if not isinstance(document, dict):
        return False

    metadata = document.get("metadata")

    if not isinstance(metadata, dict):
        return False

    if metadata.get("schemaVersion") != schema_version:
        return False

    if source_name is None:
        if metadata.get("sourceType") != "embedded":
            return False

        if metadata.get("sourceFile") is not None:
            return False

        source_hash = metadata.get("sourceSha256")

        return (
            isinstance(source_hash, str)
            and bool(source_hash)
            and source_hash == embedded_source_hash
        )

    if metadata.get("sourceFile") != source_name:
        return False

    source_hash = metadata.get("sourceSha256")

    if not isinstance(source_hash, str) or not source_hash:
        return False

    # A previously generated file can remain valid when the user returns with
    # only the source files that still need attention. If the source is present,
    # however, its exact contents must still match the generated JSON.
    if source_path is None or not source_path.is_file():
        return True

    try:
        return source_hash == sha256_file(source_path)
    except OSError:
        return False


def build_all(source_dir: Path, output_dir: Path):
    source_dir = Path(source_dir)
    output_dir = Path(output_dir)

    if not source_dir.is_dir():
        return {
            "success": False,
            "error": "Source directory does not exist.",
            "results": [],
        }

    try:
        output_dir.mkdir(parents=True, exist_ok=True)
    except OSError:
        traceback.print_exc(file=sys.stderr)
        return {
            "success": False,
            "error": "Output directory could not be created.",
            "results": [],
        }

    if not output_dir.is_dir():
        return {
            "success": False,
            "error": "Output path is not a directory.",
            "results": [],
        }

    results = []

    for name, module, builder in BUILDERS:
        source_name = module.SOURCE_FILE
        output_name = module.OUTPUT_FILE
        schema_version = module.SCHEMA_VERSION
        embedded_source_hash = getattr(module, "SOURCE_SHA256", None)
        source_label = source_name if source_name is not None else "embedded"

        source_path = source_dir / source_name if source_name is not None else None
        output_path = output_dir / output_name

        if output_is_current(
            output_path,
            source_path,
            source_name,
            schema_version,
            embedded_source_hash,
        ):
            results.append(
                {
                    "name": name,
                    "source": source_label,
                    "success": True,
                    "status": "current",
                    "schemaVersion": schema_version,
                    "output": str(output_path),
                }
            )
            continue

        try:
            success, message = builder(source_dir, output_dir)
        except Exception:
            traceback.print_exc(file=sys.stderr)
            results.append(
                {
                    "name": name,
                    "source": source_label,
                    "success": False,
                    "schemaVersion": schema_version,
                    "error": f"{source_label} could not be processed.",
                }
            )
            continue

        if not success:
            results.append(
                {
                    "name": name,
                    "source": source_label,
                    "success": False,
                    "schemaVersion": schema_version,
                    "error": message,
                }
            )
            continue

        # A builder reporting success is not enough. Verify that the generated
        # file actually carries the expected schema version and source hash.
        if not output_is_current(
            output_path,
            source_path,
            source_name,
            schema_version,
            embedded_source_hash,
        ):
            results.append(
                {
                    "name": name,
                    "source": source_label,
                    "success": False,
                    "schemaVersion": schema_version,
                    "error": f"{output_name} could not be verified.",
                }
            )
            continue

        results.append(
            {
                "name": name,
                "source": source_label,
                "success": True,
                "status": "generated",
                "schemaVersion": schema_version,
                "output": message,
            }
        )

    return {
        "success": all(result["success"] for result in results),
        "results": results,
    }


def main():
    if len(sys.argv) != 3:
        result = {
            "success": False,
            "error": "Expected source and output directory arguments.",
            "results": [],
        }
        print(json.dumps(result, ensure_ascii=False))
        return 2

    source_dir = Path(sys.argv[1])
    output_dir = Path(sys.argv[2])

    try:
        result = build_all(source_dir, output_dir)
    except Exception:
        traceback.print_exc(file=sys.stderr)
        result = {
            "success": False,
            "error": "Data generation failed unexpectedly.",
            "results": [],
        }

    # stdout is the machine-readable contract consumed by C#.
    print(json.dumps(result, ensure_ascii=False))

    return 0 if result["success"] else 1


if __name__ == "__main__":
    sys.exit(main())
