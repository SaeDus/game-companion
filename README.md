# Game Companion

Game Companion is a desktop application for exporting game state into a portable JSON package that can be shared with an AI companion.

The project started with Diablo II: Resurrected and is being built so additional games can be supported through their own exporters.

The main goal is simple: keep the important state of a run in a file instead of relying on one conversation or one AI model to remember everything.

## What it does

Game Companion reads data from a supported game, converts it into a cleaner state model, and builds a `game-state.json` file around it.

For Diablo II, that currently means reading the character and stash saves, resolving useful game data, and exporting things such as:

- Character level, class, attributes, and skills
- Equipment, inventory, and stash items
- Mercenary information
- Quest progress
- Selected run resources
- Item names, stats, sockets, sets, uniques, and runewords

The desktop app handles file selection, runs the exporter, shows basic status information, and saves the final package.

As the project develops, the same package will also include the rules, instructions, mechanics, history, and other information needed to continue an AI-assisted run without depending on previous chat context.

## Current support

### Diablo II: Resurrected

The Diablo II exporter is the first working implementation and is currently the main development target.

It uses:

- C# / .NET for save parsing and formatting
- D2SSharp for reading Diablo II save structures
- Python for generating supporting lookup data
- React + TypeScript for the desktop UI
- Tauri for native desktop access

See [`exporters/diablo-2/README.md`](exporters/diablo-2/README.md) for exporter details.

## Repository layout

```text
game-companion/
├─ app/                    # React + Tauri desktop application
└─ exporters/
   └─ diablo-2/            # Diablo II save exporter and data generators
```

Additional documentation:

- [`app/README.md`](app/README.md)
- [`exporters/diablo-2/README.md`](exporters/diablo-2/README.md)

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
