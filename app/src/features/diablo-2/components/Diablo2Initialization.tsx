import { useState } from "react";
import { open } from "@tauri-apps/plugin-dialog";
import { Command } from "@tauri-apps/plugin-shell";

interface InitializationResult {
  success: boolean;
  error: string | null;
  results: InitializationItemResult[];
}

interface InitializationItemResult {
  name: string;
  source: string;
  success: boolean;
  status: "current" | "generated" | null;
  output: string | null;
  error: string | null;
}

function Diablo2Initialization() {
  const [inputPath, setInputPath] = useState<string | null>(null);

  const [error, setError] = useState<string | null>(null);
  const [status, setStatus] = useState<InitializationResult | null>(null);

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

    try {
      setError(null);
      setStatus(null);

      const result = await Command.create("d2-exporter", [
        "run",
        "--project",
        "../../exporters/diablo-2/d2-reader.csproj",
        "--",
        "initialize",
        inputPath,
      ]).execute();

      if (!result.stdout.trim()) {
        throw new Error(
          result.stderr.trim() ||
          `Diablo II exporter failed with code ${result.code}`
        );
      }

      const dataState: InitializationResult = JSON.parse(result.stdout);
      setStatus(dataState);
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
      {status?.results.map((item) => (
        <div key={item.name}>
          <span>{item.source}</span>

          {item.success ? (
            <span>{item.status}</span>
          ) : (
            <span>{item.error}</span>
          )}
        </div>
      ))}
      <pre>{error}</pre>
    </>
  );
}

export default Diablo2Initialization;
