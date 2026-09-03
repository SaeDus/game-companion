# Diablo II Exporter

This directory contains the **Diablo II: Resurrected** exporter used by Game Companion.

Its purpose is to translate native Diablo II save data and supporting game data into a **semantic, AI-friendly state model** that the desktop application can place inside a portable `game-state.json` package.

The exporter is game-specific by design. Diablo II knows how to describe Diablo II; the rest of Game Companion should not need to know how a `.d2s` file is encoded, how mercenary progression works, or how a raw item statistic becomes readable text.

## What it exports

The current exporter reads:

- Character saves (`.d2s`)
- Shared stash saves (`.d2i`)

It resolves and exports information including:

- Character name, class, and level
- Base attributes
- Unspent stat and skill points
- Learned skills
- Equipment
- Inventory
- Personal stash
- Mercenary identity and subtype
- Mercenary level, attributes, skills, and equipment
- Shared stash tabs
- Quest state for Normal, Nightmare, and Hell
- Selected controlled resources
- Chronicle data stored through the development stash workflow
- Item base types and names
- Unique and set identities
- Runeword names and stats
- Item properties and formatted statistics
- Socket contents
- Advanced stash stack quantities

The goal is not to reproduce the binary save structure as JSON. The goal is to expose the **meaningful current game state** in a form that another system can reason about.

## Architecture

```text
Diablo II saves
   .d2s / .d2i
        |
        v
     D2SSharp
        |
        v
    SaveLoader
        |
        v
Formatting layer
├─ GameFormatter
├─ CharacterFormatter
├─ ItemFormatter
├─ StatFormatter
├─ QuestLogFormatter
└─ ChronicleFormatter
        |
        +------ generated lookup data
        |       armor / weapons / skills /
        |       runes / sets / uniques / etc.
        v
Strongly typed semantic models
        |
        v
JSON written to stdout
        |
        v
Game Companion desktop app
```

### Main directories

```text
exporters/diablo-2/
├─ Export/        # JSON serialization/export behavior
├─ Formatting/    # Raw model -> semantic state transformation
├─ Lookups/       # Access to generated Diablo II metadata
├─ Models/        # State and generated-data models
├─ Parsing/       # Save loading
└─ generators/    # Python builders for supporting lookup data
```

## Requirements

Current development requirements:

- .NET SDK supporting `net10.0`
- Python 3 for lookup-data generation
- Diablo II source data files for initialization where required

The C# project currently depends on:

- [`D2SSharp`](https://www.nuget.org/packages/D2SSharp) for reading Diablo II save structures

Install/restore normal .NET dependencies with:

```bash
dotnet restore
```

## Commands

The exporter exposes a small command-line contract.

### Export character state

```bash
dotnet run --project d2-reader.csproj -- export <character.d2s> <stash.d2i>
```

On success, normalized JSON is written to **stdout**.

Diagnostic information, including currently unhandled item/stat formatting information, may be written to **stderr**.

Exit codes are used as a process contract:

```text
0  success
1  command completed but generation/initialization failed
2  invalid command or invalid arguments
```

### Initialize lookup data

```bash
dotnet run --project d2-reader.csproj -- initialize <source_dir> <output_dir>
```

Initialization launches the Python generator and produces the JSON lookup files used by the C# formatting layer.

The generator itself can also be invoked directly:

```bash
python generators/generator.py <source_dir> <output_dir>
```

Its stdout is a single machine-readable JSON result object so that the C# process can consume it reliably.

## Generated lookup data

Diablo II save files contain many identifiers that are useful to the game but not particularly useful to a human or AI in raw form. The exporter therefore relies on generated lookup data for information such as:

- Armor
- Weapons
- Miscellaneous items
- Item types
- Element types
- Player classes
- Skills
- Skill tabs
- Hirelings
- Runes and runewords
- Set definitions and set items
- Unique items

The Python generator currently coordinates 13 individual builders.

### Cache and verification model

Generated JSON files include metadata such as:

```json
{
  "metadata": {
    "schemaVersion": 1,
    "sourceFile": "example.txt",
    "sourceSha256": "..."
  },
  "data": {}
}
```

The generator uses this metadata to determine whether an existing output is still valid.

A file is regenerated when appropriate if:

- The output does not exist
- The output is invalid JSON
- Required metadata is missing or malformed
- The builder schema version has changed
- The source file has changed
- Embedded source content has changed

If a source file is present, its SHA-256 hash must match the generated metadata for the output to be considered current.

If a previously generated TXT-backed lookup is valid and the source file is no longer present, the existing output can remain usable. This supports partial repair workflows without forcing users to repeatedly provide every source table.

After a builder reports success, the coordinator verifies the generated artifact again rather than assuming that a successful return value guarantees a valid output.

## Local data location

Generated Diablo II lookup data is stored outside the repository in the user's local application data directory.

On Windows the exporter resolves its data directory from `LocalApplicationData`, under:

```text
game-companion/diablo-2/data
```

This keeps generated game metadata and user-machine state separate from source control.

## Semantic transformation examples

### Skills

The native save format exposes positional skill data and IDs. The formatter resolves those into state such as:

```json
{
  "Id": 123,
  "Name": "Skill Name",
  "Level": 7
}
```

### Items

Items are normalized into concepts such as:

- Base code and readable base name
- Resolved unique, set, or runeword name
- Quality
- Item level
- Equipment slot or inventory coordinates
- Quantity for advanced-stack items
- Defense where applicable
- Formatted statistics
- Ethereal state
- Socket contents

Items that are not useful to the current companion workflow may be filtered rather than blindly mirroring every raw record.

### Mercenaries

Mercenary state is reconstructed from save metadata plus generated hireling progression data. The exporter resolves the mercenary's identity, level, derived Strength and Dexterity, skill list, and equipment into a readable state object.

## Why Python and C#?

The split is intentional.

**C#** handles save parsing and semantic transformation because the runtime game-state exporter works naturally around D2SSharp's .NET models.

**Python** handles preprocessing of tabular supporting game data because the builders are straightforward data-transformation utilities and can remain independent of the runtime save parser.

The process boundary between them is explicit: JSON on stdout, diagnostics on stderr, and exit codes for success/failure.

## Development notes

This exporter is still under active development.

Known areas of ongoing work include:

- Additional stat formatting coverage
- Magic/rare prefix and suffix naming
- Full initialization flow through the desktop app
- Packaging Python/C# components for release rather than relying on development-time `python` and `dotnet run`
- Expanding the final Game Companion package beyond character state

## Blizzard / Diablo disclaimer

This is an **independent, unofficial, fan-made development project** and is not affiliated with, endorsed by, sponsored by, or otherwise associated with Blizzard Entertainment, Inc. or its affiliates.

**Diablo**, **Diablo II**, **Diablo II: Resurrected**, **Blizzard**, and related names, characters, logos, game data, and other intellectual property are trademarks and/or copyrighted works of their respective owners.

This repository does **not** include Blizzard's proprietary game data files. Where supporting Diablo II data is required to generate lookup information, those files must be supplied by the user from their own installation or other lawful source. Generated lookup data is stored locally rather than committed as a substitute distribution of the original game data.

Use of this project is intended for personal tooling, experimentation, interoperability, and AI-assisted gameplay with legitimately obtained game data and save files.

Nothing in this repository grants any rights to Blizzard intellectual property.

## Related documentation

- [`../../README.md`](../../README.md) - overall Game Companion project
- [`../../app/README.md`](../../app/README.md) - Tauri + React desktop application
