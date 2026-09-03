# Diablo II Exporter

This directory contains the Diablo II: Resurrected exporter used by Game Companion.

It reads Diablo II save data, resolves the parts that are useful outside the game, and exports a cleaner JSON state for the desktop app.

## Current support

The exporter reads:

- Character saves (`.d2s`)
- Shared stash saves (`.d2i`)

It currently exports:

- Character name, class, and level
- Base attributes
- Unspent stat and skill points
- Learned skills
- Equipment
- Inventory
- Personal stash
- Mercenary information and equipment
- Shared stash tabs
- Quest state for Normal, Nightmare, and Hell
- Selected controlled resources
- Chronicle data used by the current development run
- Item base names and codes
- Unique, set, and runeword names
- Item stats
- Socket contents
- Advanced stash stack quantities

The exporter is meant to describe the useful game state, not reproduce the save file structure directly.

## Structure

```text
exporters/diablo-2/
├─ Export/        # JSON serialization
├─ Formatting/    # Save data -> exported state
├─ Lookups/       # Generated Diablo II lookup data
├─ Models/        # Export and lookup models
├─ Parsing/       # Save loading
└─ generators/    # Python data builders
```

The main flow is:

```text
.d2s / .d2i
    |
    v
D2SSharp
    |
    v
SaveLoader
    |
    v
Formatting + Lookups
    |
    v
GameState
    |
    v
JSON on stdout
```

## Requirements

- .NET SDK supporting `net10.0`
- Python 3 for lookup-data generation
- Diablo II source data files when initializing lookup data

The C# project uses [D2SSharp](https://www.nuget.org/packages/D2SSharp) to read Diablo II save structures.

Restore dependencies with:

```bash
dotnet restore
```

## Commands

### Export character state

```bash
dotnet run --project d2-reader.csproj -- export <character.d2s> <stash.d2i>
```

On success, the exported JSON is written to stdout.

Diagnostics may be written to stderr.

Exit codes:

```text
0  success
1  generation or initialization failure
2  invalid command or arguments
```

### Initialize lookup data

```bash
dotnet run --project d2-reader.csproj -- initialize <source_dir> <output_dir>
```

This runs the Python generator used to build the lookup JSON files needed by the formatter.

The generator can also be run directly:

```bash
python generators/generator.py <source_dir> <output_dir>
```

Its stdout is a single JSON result object so it can be consumed by the C# process.

## Generated lookup data

Diablo II save files contain IDs and codes that need supporting game data to become readable names and values.

The current generators cover:

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
- Sets and set items
- Unique items

Generated files include metadata containing a schema version and source hash. The generator uses that metadata to decide whether an existing file can be reused or needs to be rebuilt.

Example:

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

A generated file is rebuilt when its output is missing or invalid, its schema changes, or its source data changes. Generated output is verified again after a builder reports success.

## Local data

Generated lookup files are stored outside the repository in the user's local application data directory.

On Windows:

```text
game-companion/diablo-2/data
```

This keeps generated game data out of source control.

## Development notes

The exporter is still under active development.

Current areas that still need work include:

- More item/stat formatting coverage
- Magic and rare prefix/suffix naming
- Initialization through the desktop app
- Release packaging for the C# and Python components
- Additional state needed by the full Game Companion package

## Blizzard / Diablo disclaimer

This is an independent, unofficial fan project. It is not affiliated with, endorsed by, sponsored by, or associated with Blizzard Entertainment, Inc. or its affiliates.

Diablo, Diablo II, Diablo II: Resurrected, Blizzard, and related names, characters, logos, game data, and other intellectual property belong to their respective owners.

This repository does not include Blizzard's proprietary game data files. Any supporting Diablo II data required to build lookup files must be supplied by the user from their own installation or another lawful source. Generated lookup data is stored locally rather than committed to the repository.

Nothing in this repository grants rights to Blizzard intellectual property.

## Related documentation

- [`../../README.md`](../../README.md)
- [`../../app/README.md`](../../app/README.md)
