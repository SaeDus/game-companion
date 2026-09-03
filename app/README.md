# Game Companion Desktop App

This directory contains the desktop application for **Game Companion**.

The app is built with **React + TypeScript** and packaged with **Tauri 2**, giving the project a lightweight native desktop shell while keeping the UI and package-building workflow in the web stack.

Its job is intentionally different from the game-specific exporters: the app coordinates user input, native file access, exporter execution, package assembly, and output. It should not need to understand the internal save format of every supported game.

## Responsibilities

The desktop application currently handles:

- Selecting game save sources through native file dialogs
- Persisting configured source paths between sessions
- Launching the Diablo II exporter through Tauri's shell plugin
- Reading machine-readable exporter output
- Displaying generation status and exporter diagnostics
- Building the top-level `game-state.json` package
- Previewing detected character information
- Saving generated state through a native save dialog

As additional games are added, the long-term goal is to keep these product-level responsibilities shared while isolating game-specific configuration and exporter behavior behind game-specific features/adapters.

## Stack

```text
React 19
TypeScript
Vite
Tailwind CSS
Lucide React
        |
        v
Tauri 2
        |
        +-- Dialog plugin
        +-- File-system plugin
        +-- Shell plugin
        +-- Store plugin
        +-- Opener plugin
```

### React + TypeScript

The React layer owns the visible workflow and package orchestration.

Current feature areas include:

```text
src/features/
├─ Diablo2/       # Diablo II-specific UI, components, and state types
└─ GameState/     # Top-level game-state generation workflow
```

`GameStateBuilder` currently drives the active application flow. It selects save sources, calls the Diablo II exporter, parses the normalized state returned on stdout, combines it with top-level metadata, and writes the finished JSON package.

The current implementation is still transitioning from a Diablo II-specific prototype toward a multi-game architecture, so some Diablo-specific behavior remains inside the shared builder. A future second game will help determine which adapter boundaries are genuinely reusable rather than prematurely abstracting around a single implementation.

### Tauri

Tauri provides the native desktop boundary.

The Rust layer is intentionally thin. Game semantics remain outside the Tauri host; Rust is currently responsible primarily for application startup and enabling the native capabilities used by the TypeScript frontend.

Enabled plugins include:

- `tauri-plugin-dialog`
- `tauri-plugin-fs`
- `tauri-plugin-shell`
- `tauri-plugin-store`
- `tauri-plugin-opener`

The application capability configuration explicitly permits the frontend to invoke the configured `dotnet` exporter command and to write generated text files.

Keeping this layer small avoids duplicating game-specific parsing or transformation logic inside the desktop host.

## Development

### Requirements

For the current Diablo II development workflow, the machine needs:

- Node.js / npm
- Rust toolchain required by Tauri
- .NET SDK compatible with the Diablo II exporter (`net10.0` at the time of writing)
- Python for Diablo II lookup-data initialization

Install JavaScript dependencies:

```bash
npm install
```

Run the Tauri development application:

```bash
npm run tauri dev
```

Build the frontend only:

```bash
npm run build
```

The current development configuration launches the Diablo II C# exporter through `dotnet run`, pointing at:

```text
../exporters/diablo-2/d2-reader.csproj
```

This is a development-time arrangement. A packaged release can later replace the development command with compiled sidecars/binaries without changing the higher-level game-state workflow.

## Settings

The application uses Tauri's store plugin to persist local configuration such as selected character and stash paths.

These settings are user-specific application data and are separate from the generated Diablo II lookup data used by the exporter.

## Package-building boundary

The app expects a game-specific exporter to return **normalized semantic JSON**, not raw save-file structures.

For the Diablo II prototype:

```text
.d2s + .d2i
    |
    v
C# Diablo II exporter
    |
    v
normalized CharacterState JSON
    |
    v
Game Companion desktop app
    |
    +-- Metadata
    +-- Game identity
    +-- CharacterState
    +-- future rules / mechanics / journey / run history
    |
    v
game-state.json
```

This boundary is central to the project. The desktop application should be able to work with additional games without learning how their native saves are encoded.

## Current status

The UI is functional but still under active development.

Near-term work includes:

1. Adding the Diablo II initialization/bootstrap workflow to the UI.
2. Moving persistent run rules and instructions into structured project data.
3. Expanding the final package beyond character state.
4. Separating remaining Diablo-specific configuration from the shared game-state workflow where the architecture proves it is useful.

## Related documentation

- [`../README.md`](../README.md) - overall Game Companion architecture and goals
- [`../exporters/diablo-2/README.md`](../exporters/diablo-2/README.md) - Diablo II exporter and lookup-data pipeline

## Project status

Game Companion is an experimental, actively developed personal project. The current desktop application should be treated as a development build rather than a polished end-user release.
