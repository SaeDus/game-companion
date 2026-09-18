import { useState } from "react";
import { open } from "@tauri-apps/plugin-dialog";
import { Command } from "@tauri-apps/plugin-shell";
import { localDataDir, join } from "@tauri-apps/api/path";

function Diablo2Initialization() {
  const [inputPath, setInputPath] = useState<string | null>(null);

  const [error, setError] = useState<string | null>(null);
  const [statusMessage, setStatusMessage] = useState<string>("Data Status Pending...");

  async function selectInputPath() {
    try {
      setError(null);

      const path = await open({
        multiple: false,
        directory: true,
        title: "Location of .txt data files",
      });

      if (!path) {
        return;
      }

      setInputPath(path);
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to select directory.");
      }
    }
  }

  async function initializeData() {
    if (!inputPath) {
      setError("Select a data directory first.");
      return;
    }

    const dataPath = await join(
      await localDataDir(),
      "game-companion",
      "games",
      "diablo-2",
      "data"
    );

    try {
      setError(null);

      const result = await Command.create("d2-exporter", [
        "run",
        "--project",
        "../../exporters/diablo-2/d2-reader.csproj",
        "--",
        "initialize",
        inputPath,
        dataPath,
      ]).execute();

      if (result.stderr.trim()) {
        throw new Error(
          result.stderr ||
          `Diablo II exporter failed with code ${result.code}`
        );
      }

      if (!result.stdout.trim()) {
        throw new Error(
          "Diablo II exporter returned no state data"
        );
      }

      let dataState: Record<string, unknown>;

      try {
        dataState = JSON.parse(result.stdout);
      } catch {
        console.error("Exporder stdout:", result.stdout);
        console.error("Exporter stderr:", result.stderr);

        throw new Error(
          "Diablo II exporter returned invalid JSON"
        );
      }

      const json = JSON.stringify(dataState, null, 2);

      setStatusMessage(json);
    } catch (error) {
      console.error(error);

      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError(String(error));
      }
    }
  }

  return (
    <>
      <p>Initialize Diablo II</p>
      <button
        className="select-data-button"
        onClick={selectInputPath}
      >
        Select Data Path
      </button>
      <button
        className="initialize-button"
        onClick={initializeData}
      >
        Initialize Diablo II Data
      </button>
      <p></p>
      <pre>{statusMessage}</pre>
      <p></p>
      <pre>{error}</pre>
    </>
  );
}

export default Diablo2Initialization;
