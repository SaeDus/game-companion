import { useEffect, useRef, useState } from "react";
import { open, save } from "@tauri-apps/plugin-dialog";
import { writeTextFile } from "@tauri-apps/plugin-fs";
import { Command } from "@tauri-apps/plugin-shell";
import { load, type Store } from "@tauri-apps/plugin-store";

import {
  Archive,
  CheckCircle2,
  Database,
  FolderOpen,
  LoaderCircle,
  Terminal,
  UserRound,
  AlertTriangle,
} from "lucide-react";

import type { Metadata } from "./Metadata";
import type { GameData } from "./GameData";

import "../../App.css";

function GameStateBuilder() {
  const [characterPath, setCharacterPath] = useState<string | null>(null);
  const [stashPath, setStashPath] = useState<string | null>(null);

  const [error, setError] = useState<string | null>(null);
  const [exporterMessages, setExporterMessages] = useState<string | null>(null);

  const storeRef = useRef<Store | null>(null);

  const [statusMessage, setStatusMessage] = useState<string>(
    "Configure the Diablo II save sources to begin."
  );

  const [isGenerating, setIsGenerating] = useState(false);

  const [characterPreview, setCharacterPreview] =
    useState<{
      name: string;
      className: string;
      level: number | null;
    } | null>(null);

  const metadata: Metadata = {
    Schema: "game-state",
    Version: 1,
  };

  const game: GameData = {
    Id: "diablo2-resurrected",
    Title: "Diablo II: Resurrected",
    Content: {
      Id: "reign-of-the-warlock",
      Title: "Reign of the Warlock",
      Type: "Official",
    },
  };

  useEffect(() => {
    async function initializeStore() {
      const store = await load("settings.json");

      storeRef.current = store;

      const savedCharacterPath =
        await store.get<string>("characterPath");

      const savedStashPath =
        await store.get<string>("stashPath");

      setCharacterPath(savedCharacterPath ?? null);
      setStashPath(savedStashPath ?? null);
    }

    initializeStore();
  }, []);

  async function selectCharacterPath() {
    try {
      setError(null);

      const filePath = await open({
        multiple: false,
        directory: false,
        title: "Select Character Save",
        filters: [
          {
            name: "D2S",
            extensions: ["d2s"],
          },
        ],
      });

      if (!filePath) {
        return;
      }

      setCharacterPath(filePath);
      await storeRef.current?.set("characterPath", filePath);

      setStatusMessage("Character save configured.");
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to select character .d2s file");
      }
    }
  }

  async function selectStashPath() {
    try {
      setError(null);

      const filePath = await open({
        multiple: false,
        directory: false,
        title: "Select Stash Save",
        filters: [
          {
            name: "D2I",
            extensions: ["d2i"],
          },
        ],
      });

      if (!filePath) {
        return;
      }

      setStashPath(filePath);
      await storeRef.current?.set("stashPath", filePath);

      setStatusMessage("Shared stash configured.");
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to select stash .d2i file");
      }
    }
  }

  async function createGameState() {
    if (!characterPath) {
      setError("Select a character .d2s save file first.");
      return;
    }

    if (!stashPath) {
      setError("Select a stash .d2i save file first.");
      return;
    }

    try {
      setError(null);
      setExporterMessages(null);
      setIsGenerating(true);
      setStatusMessage("Reading Diablo II save data...");

      const result = await Command.create("d2-exporter", [
        "run",
        "--project",
        "../../exporters/diablo-2/d2-reader.csproj",
        "--",
        "export",
        characterPath,
        stashPath,
      ]).execute();

      // const result = await Command.create("d2-exporter", [
      //   "run",
      //   "--project",
      //   "../../exporters/diablo-2/d2-reader.csproj",
      //   "--",
      //   "initialize",
      //   "<path_to_source_files>",
      //   "<path_to_data_directory>",
      // ]).execute();

      if (result.stderr.trim()) {
        setExporterMessages(result.stderr);
      }

      if (result.code !== 0) {
        throw new Error(
          result.stderr ||
          `Diablo II exporter failed with code ${result.code}`
        );
      }

      if (!result.stdout.trim()) {
        throw new Error(
          "Diablo II exporter returned no character data."
        );
      }

      let characterState: Record<string, unknown>;

      try {
        characterState = JSON.parse(result.stdout);
      } catch {
        console.error("Exporter stdout:", result.stdout);
        console.error("Exporter stderr:", result.stderr);

        throw new Error(
          "Diablo II exporter returned invalid JSON."
        );
      }

      setCharacterPreview(
        getCharacterPreview(characterState)
      );

      // Build the new object.
      const gameState = {
        Metadata: metadata,
        Game: game,
        CharacterState: characterState,
      };

      // Convert the completed object back into JSON text.
      const json = JSON.stringify(gameState, null, 2);

      // Ask where the new file should be created.
      const outputPath = await save({
        title: "Save Game State",
        defaultPath: "game-state.json",
        filters: [
          {
            name: "JSON",
            extensions: ["json"],
          },
        ],
      });

      if (!outputPath) {
        return;
      }

      await writeTextFile(outputPath, json);

      setStatusMessage(
        "Game state generated successfully."
      );
    } catch (error) {
      console.error(error);

      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError(String(error));
      }

      setStatusMessage("Game state generation failed.");
    } finally {
      setIsGenerating(false);
    }
  }

  return (
    <main className="app-shell min-h-screen text-zinc-100">
      <div className="app-grid pointer-events-none fixed inset-0 opacity-40" />

      <div className="relative mx-auto flex min-h-screen w-full max-w-6xl flex-col px-6 py-8 lg:px-10 lg:py-10">

        {/* Header */}
        <header className="mb-8 flex flex-col gap-5 border-b border-white/8 pb-7 md:flex-row md:items-end md:justify-between">
          <div>
            <div className="mb-3 flex items-center gap-2">
              <span className="rounded-full border border-amber-400/20 bg-amber-400/8 px-3 py-1 font-mono text-[10px] font-semibold uppercase tracking-[0.24em] text-amber-300">
                Game State Utility
              </span>

              <span
                className={`flex items-center gap-2 rounded-full border px-3 py-1 text-xs ${characterPath && stashPath
                  ? "border-emerald-400/20 bg-emerald-400/8 text-emerald-300"
                  : "border-zinc-700 bg-zinc-900/70 text-zinc-500"
                  }`}
              >
                <span
                  className={`h-1.5 w-1.5 rounded-full ${characterPath && stashPath
                    ? "bg-emerald-400 shadow-[0_0_8px_rgba(52,211,153,.8)]"
                    : "bg-zinc-600"
                    }`}
                />

                {characterPath && stashPath
                  ? "Ready"
                  : "Configuration Required"}
              </span>
            </div>

            <h1 className="text-4xl font-semibold tracking-[-0.04em] text-white sm:text-5xl">
              Game Companion
              <span className="ml-3 text-amber-400">
                State Forge
              </span>
            </h1>

            <p className="mt-3 max-w-2xl text-sm leading-6 text-zinc-400">
              Transform live game data into a portable,
              AI-ready game state package.
            </p>
          </div>

          <div className="hidden items-center gap-3 text-right md:flex">
            <Database className="h-5 w-5 text-zinc-600" />

            <div>
              <p className="font-mono text-[10px] uppercase tracking-[0.2em] text-zinc-600">
                Active Profile
              </p>

              <p className="mt-1 text-sm font-medium text-zinc-300">
                Diablo II: Resurrected
              </p>
            </div>
          </div>
        </header>

        {/* Main workspace */}
        <div className="grid flex-1 gap-6 xl:grid-cols-[1fr_340px]">

          {/* Left column */}
          <section className="space-y-6">

            {/* Input configuration */}
            <div className="glass-panel rounded-2xl border border-white/8 bg-zinc-950/65 p-6 backdrop-blur-xl">
              <div className="mb-6 flex items-center justify-between">
                <div>
                  <p className="font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-amber-400">
                    Source Configuration
                  </p>

                  <h2 className="mt-1 text-xl font-semibold text-zinc-100">
                    Diablo II save files
                  </h2>
                </div>

                <span className="font-mono text-xs text-zinc-600">
                  {[characterPath, stashPath].filter(Boolean).length}/2
                </span>
              </div>

              <div className="space-y-4">

                {/* Character save */}
                <div className="group rounded-xl border border-white/8 bg-white/[0.025] p-4 transition hover:border-amber-400/20 hover:bg-white/[0.04]">
                  <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl border border-amber-400/15 bg-amber-400/8 text-amber-300">
                      <UserRound className="h-5 w-5" />
                    </div>

                    <div className="min-w-0 flex-1">
                      <div className="mb-1 flex items-center gap-2">
                        <h3 className="text-sm font-semibold text-zinc-200">
                          Character Save
                        </h3>

                        <span className="font-mono text-[10px] uppercase text-zinc-600">
                          .d2s
                        </span>

                        {characterPath && (
                          <CheckCircle2 className="h-3.5 w-3.5 text-emerald-400" />
                        )}
                      </div>

                      <p
                        className={`break-all font-mono text-xs leading-5 ${characterPath
                          ? "text-zinc-400"
                          : "text-zinc-700"
                          }`}
                      >
                        {characterPath || "No character save selected"}
                      </p>
                    </div>

                    <button
                      type="button"
                      onClick={selectCharacterPath}
                      className="inline-flex shrink-0 items-center justify-center gap-2 rounded-lg border border-white/10 bg-white/[0.045] px-4 py-2.5 text-sm font-medium text-zinc-300 transition hover:border-amber-400/25 hover:bg-amber-400/10 hover:text-amber-200 active:scale-[0.98]"
                    >
                      <FolderOpen className="h-4 w-4" />

                      {characterPath ? "Change" : "Select"}
                    </button>
                  </div>
                </div>

                {/* Stash */}
                <div className="group rounded-xl border border-white/8 bg-white/[0.025] p-4 transition hover:border-amber-400/20 hover:bg-white/[0.04]">
                  <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl border border-violet-400/15 bg-violet-400/8 text-violet-300">
                      <Archive className="h-5 w-5" />
                    </div>

                    <div className="min-w-0 flex-1">
                      <div className="mb-1 flex items-center gap-2">
                        <h3 className="text-sm font-semibold text-zinc-200">
                          Shared Stash
                        </h3>

                        <span className="font-mono text-[10px] uppercase text-zinc-600">
                          .d2i
                        </span>

                        {stashPath && (
                          <CheckCircle2 className="h-3.5 w-3.5 text-emerald-400" />
                        )}
                      </div>

                      <p
                        className={`break-all font-mono text-xs leading-5 ${stashPath
                          ? "text-zinc-400"
                          : "text-zinc-700"
                          }`}
                      >
                        {stashPath || "No stash save selected"}
                      </p>
                    </div>

                    <button
                      type="button"
                      onClick={selectStashPath}
                      className="inline-flex shrink-0 items-center justify-center gap-2 rounded-lg border border-white/10 bg-white/[0.045] px-4 py-2.5 text-sm font-medium text-zinc-300 transition hover:border-violet-400/25 hover:bg-violet-400/10 hover:text-violet-200 active:scale-[0.98]"
                    >
                      <FolderOpen className="h-4 w-4" />

                      {stashPath ? "Change" : "Select"}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* Generate */}
            <div className="relative overflow-hidden rounded-2xl border border-amber-400/15 bg-gradient-to-br from-amber-400/[0.08] via-zinc-950/80 to-zinc-950/80 p-6">
              <div className="absolute -right-16 -top-24 h-64 w-64 rounded-full bg-amber-500/8 blur-3xl" />

              <div className="relative flex flex-col gap-5 md:flex-row md:items-center md:justify-between">
                <div>
                  <p className="font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-amber-400">
                    State Generation
                  </p>

                  <h2 className="mt-1 text-xl font-semibold text-zinc-100">
                    Build the complete game package
                  </h2>

                  <p className="mt-2 max-w-xl text-sm leading-6 text-zinc-500">
                    Read the configured saves, normalize the
                    character data, and generate the final
                    game-state.json package.
                  </p>
                </div>

                <button
                  type="button"
                  onClick={createGameState}
                  disabled={
                    !characterPath ||
                    !stashPath ||
                    isGenerating
                  }
                  className="generate-button inline-flex min-w-48 items-center justify-center gap-2 rounded-xl bg-amber-400 px-6 py-3.5 text-sm font-bold text-zinc-950 shadow-[0_0_30px_rgba(251,191,36,.12)] transition enabled:hover:bg-amber-300 enabled:hover:shadow-[0_0_35px_rgba(251,191,36,.22)] enabled:active:scale-[0.98] disabled:cursor-not-allowed disabled:bg-zinc-800 disabled:text-zinc-600 disabled:shadow-none"
                >
                  {isGenerating ? (
                    <>
                      <LoaderCircle className="h-4 w-4 animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Database className="h-4 w-4" />
                      Generate Game State
                    </>
                  )}
                </button>
              </div>
            </div>

            {/* Diagnostics */}
            <div className="glass-panel overflow-hidden rounded-2xl border border-white/8 bg-zinc-950/65 backdrop-blur-xl">
              <div className="flex items-center justify-between border-b border-white/8 px-5 py-4">
                <div className="flex items-center gap-3">
                  <Terminal className="h-4 w-4 text-zinc-500" />

                  <div>
                    <h2 className="text-sm font-semibold text-zinc-300">
                      Exporter Diagnostics
                    </h2>

                    <p className="text-xs text-zinc-600">
                      stderr output from the active exporter
                    </p>
                  </div>
                </div>

                <span
                  className={`rounded-full px-2 py-1 font-mono text-[9px] uppercase tracking-wider ${exporterMessages
                    ? "bg-amber-400/10 text-amber-400"
                    : "bg-zinc-900 text-zinc-600"
                    }`}
                >
                  {exporterMessages ? "Output" : "Quiet"}
                </span>
              </div>

              <pre className="diagnostic-scroll min-h-36 max-h-72 overflow-auto whitespace-pre-wrap break-words p-5 font-mono text-xs leading-6 text-zinc-500">
                {exporterMessages ||
                  "No exporter diagnostics reported."}
              </pre>
            </div>
          </section>

          {/* Right sidebar */}
          <aside className="space-y-6">

            {/* Character snapshot */}
            <div className="glass-panel rounded-2xl border border-white/8 bg-zinc-950/65 p-6 backdrop-blur-xl">
              <div className="mb-5 flex items-center justify-between">
                <div>
                  <p className="font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-violet-400">
                    Character Snapshot
                  </p>

                  <h2 className="mt-1 text-lg font-semibold text-zinc-200">
                    Last generated state
                  </h2>
                </div>

                <UserRound className="h-5 w-5 text-zinc-700" />
              </div>

              {characterPreview ? (
                <div>
                  <div className="mb-5 rounded-xl border border-white/8 bg-white/[0.025] p-5">
                    <p className="text-2xl font-semibold tracking-tight text-white">
                      {characterPreview.name}
                    </p>

                    <div className="mt-2 flex items-center gap-2 text-sm">
                      <span className="text-violet-300">
                        {characterPreview.className}
                      </span>

                      {characterPreview.level !== null && (
                        <>
                          <span className="text-zinc-700">/</span>

                          <span className="text-zinc-400">
                            Level {characterPreview.level}
                          </span>
                        </>
                      )}
                    </div>
                  </div>

                  <div className="flex items-center gap-2 text-xs text-emerald-400">
                    <CheckCircle2 className="h-4 w-4" />
                    Character state loaded
                  </div>
                </div>
              ) : (
                <div className="flex min-h-36 flex-col items-center justify-center rounded-xl border border-dashed border-white/8 bg-white/[0.015] px-5 text-center">
                  <UserRound className="mb-3 h-7 w-7 text-zinc-800" />

                  <p className="text-sm font-medium text-zinc-600">
                    No character loaded
                  </p>

                  <p className="mt-1 text-xs leading-5 text-zinc-700">
                    Generate a game state to populate this
                    snapshot.
                  </p>
                </div>
              )}
            </div>

            {/* Status */}
            <div className="glass-panel rounded-2xl border border-white/8 bg-zinc-950/65 p-6 backdrop-blur-xl">
              <p className="mb-4 font-mono text-[10px] font-semibold uppercase tracking-[0.2em] text-zinc-500">
                System Status
              </p>

              <div
                className={`rounded-xl border p-4 ${error
                  ? "border-red-400/15 bg-red-400/[0.05]"
                  : "border-white/8 bg-white/[0.025]"
                  }`}
              >
                <div className="flex items-start gap-3">
                  {error ? (
                    <AlertTriangle className="mt-0.5 h-4 w-4 shrink-0 text-red-400" />
                  ) : (
                    <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0 text-emerald-400" />
                  )}

                  <div className="min-w-0">
                    <p
                      className={`text-sm font-medium ${error
                        ? "text-red-300"
                        : "text-zinc-300"
                        }`}
                    >
                      {error ? "Action required" : "Game Companion"}
                    </p>

                    <p
                      className={`mt-1 break-words text-xs leading-5 ${error
                        ? "text-red-300/70"
                        : "text-zinc-500"
                        }`}
                    >
                      {error || statusMessage}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            {/* Package info */}
            <div className="rounded-2xl border border-white/6 bg-white/[0.02] p-5">
              <p className="font-mono text-[10px] uppercase tracking-[0.18em] text-zinc-700">
                Output Package
              </p>

              <div className="mt-3 flex items-center gap-3">
                <Database className="h-4 w-4 text-amber-500/70" />

                <div>
                  <p className="font-mono text-xs text-zinc-400">
                    game-state.json
                  </p>

                  <p className="mt-1 text-[11px] text-zinc-700">
                    Schema version {metadata.Version}
                  </p>
                </div>
              </div>
            </div>
          </aside>
        </div>

        <footer className="mt-8 border-t border-white/5 pt-5 text-center font-mono text-[10px] uppercase tracking-[0.16em] text-zinc-800">
          Game Companion / Local State Generation
        </footer>
      </div>
    </main>
  );
}

function getCharacterPreview(
  characterState: Record<string, unknown>
) {
  const character = characterState.Character;

  if (
    !character ||
    typeof character !== "object" ||
    Array.isArray(character)
  ) {
    return null;
  }

  const data = character as Record<string, unknown>;

  return {
    name:
      typeof data.Name === "string"
        ? data.Name
        : "Unknown Character",

    className:
      typeof data.Class === "string"
        ? data.Class
        : "Unknown Class",

    level:
      typeof data.Level === "number"
        ? data.Level
        : null,
  };
}

export default GameStateBuilder;
