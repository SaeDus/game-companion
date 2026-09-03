# Game Companion

Game Companion is a desktop utility for turning live game data into portable, structured game-state packages that can be used by an AI companion without depending on prior conversation memory.

The project began as a Diablo II: Resurrected prototype, but its architecture is intended to support additional games through game-specific exporters and a shared desktop application.

> **Core idea:** the run should live in the artifact, not in the AI's memory.

## What it does

Game Companion sits between a game and an AI assistant.

```text
Game save / game data
        |
        v
Game-specific exporter
        |
        v
Normalized semantic state
        |
        v
Game Companion package builder
        |
        v
game-state.json
        |
        v
AI companion
```

Instead of handing an AI raw save data, Game Companion translates game-specific structures into human- and machine-readable state such as character attributes, skills, equipment, quests, resources, companions, inventory, and other information relevant to the current run.

The long-term goal is a self-contained snapshot that can preserve the quality and continuity of an AI-assisted playthrough even when changing models, services, conversations, or devices.

## Current prototype: Diablo II: Resurrected

The first supported game is **Diablo II: Resurrected**, using a modded character as the project's primary development case.

The Diablo II pipeline currently supports:

- Character save (`.d2s`) parsing
- Shared stash (`.d2i`) parsing
- Character attributes and unspent points
- Learned skills
- Equipped items, inventory, and personal stash
- Mercenary identity, level, attributes, skills, and equipment
- Shared stash tabs
- Quest state across difficulties and acts
- Selected controlled resources
- Chronicle data derived from the stash
- Item names, base types, sockets, runewords, sets, uniques, and formatted item stats
- Generated lookup data sourced from Diablo II data tables

The desktop application can select save sources, persist their paths, invoke the exporter, preview the detected character, assemble the package, and write the resulting `game-state.json`.

## Architecture

Game Companion intentionally uses different technologies at different boundaries instead of forcing the entire pipeline into one language.

```text
React + TypeScript
    UI and package orchestration

Tauri + Rust
    Desktop application boundary and native capabilities

C# / .NET
    Game-specific save parsing and semantic transformation

Python
    Generation and validation of supporting lookup data
```

### Repository layout

```text
game-companion/
├─ app/
│  ├─ src/                 # React + TypeScript UI
│  └─ src-tauri/           # Tauri desktop host
│
└─ exporters/
   └─ diablo-2/
      ├─ Export/           # JSON export behavior
      ├─ Formatting/       # Raw-save -> semantic-state translation
      ├─ Lookups/          # Generated game-data lookup access
      ├─ Models/           # Exported state models
      ├─ Parsing/          # Save loading
      └─ generators/       # Python lookup-data builders
```

More detailed documentation lives alongside each major part of the project:

- [`app/README.md`](app/README.md) - desktop application, React UI, and Tauri integration
- [`exporters/diablo-2/README.md`](exporters/diablo-2/README.md) - Diablo II exporter, data generation, and command-line usage

## Design principles

### Semantic state, not raw data

The exporter is not intended to mirror a save file byte-for-byte. It translates game-specific data into concepts useful to a player and an AI.

For example, a raw item record becomes an item with a resolved base name, quality, location, equipment slot, stats, sockets, and other meaningful properties. Positional skill records become named learned skills. Mercenary progression data becomes a resolved companion state.

### Self-contained runs

A finished package is intended to carry enough information for a capable AI to understand what game is being played, what the current state is, what rules govern the run, and what has happened so far without requiring hidden model memory.

The current public prototype is still building toward that complete package. Character state export is functional; persistent rules, run history, mechanics, and other portable context are planned additions to the same artifact.

### Game-specific adapters, shared companion

Every game exposes useful state differently. Diablo II uses binary saves and supporting data tables; another game may expose JSON, an API, a planner export, or require an entirely different adapter.

Game Companion keeps that extraction logic game-specific while allowing the desktop application and final package concept to remain shared.

### Verify generated data

The Diablo II data generator records schema versions and SHA-256 hashes for generated lookup files. Existing outputs can be reused when they are still valid, rebuilt when their source changes, and independently verified after generation.

This keeps generated game metadata reproducible without committing the source game data itself to the repository.

## Development status

Game Companion is under active development and is currently an early prototype rather than a finished end-user release.

Current work is focused on completing the Diablo II vertical slice:

1. Bootstrap required game lookup data through the application.
2. Export and normalize live character state.
3. Add portable rules, instructions, mechanics, and run history.
4. Assemble a self-contained AI-ready `game-state.json`.
5. Use a second supported game to validate which abstractions are truly reusable.

## Why this project exists

AI-assisted gameplay is usually conversation-bound. Important rules, discoveries, character state, and decisions can disappear when context is lost or when a player changes models or services.

Game Companion explores a different approach: make the **artifact** authoritative.

That turns the AI into a replaceable interpreter of a durable run state instead of making the conversation itself the only place the run can exist.

## Portfolio notes

From an engineering perspective, the project is an exercise in translating between several representations and execution environments:

- Binary game saves
- External game-data tables
- Third-party parsing models
- Generated lookup databases
- Strongly typed semantic state
- Desktop process boundaries
- Portable JSON contracts
- AI-facing context packages

The emphasis is on explicit boundaries and inspectable data contracts rather than embedding game-specific assumptions throughout the desktop application.

## Disclaimer

Game Companion is an independent, unofficial project. Support for individual games does not imply affiliation with or endorsement by their developers or publishers.

Game-specific trademark and legal notices are documented with the corresponding exporter.
