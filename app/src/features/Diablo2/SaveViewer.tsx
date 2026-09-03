import { useState } from "react";
import { open } from "@tauri-apps/plugin-dialog";
import { readTextFile } from "@tauri-apps/plugin-fs";

import type { GameState } from "./types/gameState";

import CharacterSummary from "./components/CharacterSummary";
import AttributesPanel from "./components/AttributesPanel";
import SkillsList from "./components/SkillsList";
import EquipmentPanel from "./components/EquipmentPanel";
import InventoryPanel from "./components/InventoryPanel";

import MercenarySummary from "./components/MercenarySummary";
import MercenaryEquipmentPanel from "./components/MercenaryEquipmentPanel";

import StashPanel from "./components/StashPanel";
import QuestLogPanel from "./components/QuestLogPanel";

import "./SaveViewer.css";

function SaveViewer() {
  const [gameState, setGameState] = useState<GameState | null>(null);
  const [selectedFile, setSelectedFile] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadCharacterState() {
    try {
      setError(null);

      const filePath = await open({
        multiple: false,
        directory: false,
        title: "Select Character State",
        filters: [
          {
            name: "JSON",
            extensions: ["json"],
          },
        ],
      });

      if (!filePath) {
        return;
      }

      const contents = await readTextFile(filePath);
      const data: GameState = JSON.parse(contents);

      setGameState(data);
      setSelectedFile(filePath);
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to load character state.");
      }
    }
  }

  async function refreshCharacterState() {
    if (!selectedFile) {
      return;
    }

    try {
      setError(null);

      const contents = await readTextFile(selectedFile);
      const data: GameState = JSON.parse(contents);

      setGameState(data);
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to refresh character state.");
      }
    }
  }

  if (!gameState) {
    return (
      <main>
        <h1>Game Companion</h1>

        <button onClick={loadCharacterState}>
          Load Character State
        </button>

        {error && <p>{error}</p>}
      </main>
    );
  }

  const character = gameState.Character;
  const mercenary = gameState.Mercenary;

  return (
    <main>
      <div>
        <button
          onClick={refreshCharacterState}
          disabled={!selectedFile}
        >
          Refresh
        </button>

        <button onClick={loadCharacterState}>
          Load Another Character
        </button>
      </div>

      {selectedFile && <p>{selectedFile}</p>}

      {error && <p>{error}</p>}

      <div>
        <CharacterSummary character={character} />
        <AttributesPanel character={character} />
        <SkillsList character={character} />
        <EquipmentPanel character={character} />
        <InventoryPanel character={character} />
      </div>

      <div>
        <MercenarySummary mercenary={mercenary} />
        <MercenaryEquipmentPanel mercenary={mercenary} />
      </div>

      <div>
        <StashPanel stash={gameState.Stash} />
      </div>

      <div>
        <QuestLogPanel questLog={gameState.QuestLog} />
      </div>
    </main>
  );
}

export default SaveViewer;
