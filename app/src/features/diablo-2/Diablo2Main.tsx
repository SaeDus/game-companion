import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import { Objective } from "./types/RuntimeInstructions.ts";

function Diablo2Main() {
  const [runtimeInstructions, setRuntimeInstructions] = useState("");
  const [objective, setObjective] = useState<Objective | null>(null);
  
  const currentTask = 
    objective?.Rules.find(
      (rule) => rule.Id === "current-task"
    )?.Content ?? [];
  
  const currentStrategy =
    objective?.Rules.find(
      (rule) => rule.Id === "strategy"
    )?.Content ?? [];
  
  const currentEarlyReportConditions =
    objective?.Rules.find(
      (rule) => rule.Id === "early-report-conditions"
    )?.Content ?? [];

  useEffect(() => {
    async function initializeRuntimeInstructions() {
      try {
        const result = await invoke<string>(
          "get_runtime_instructions",
          {
            characterId: "vaelric",
          }
        );

        setRuntimeInstructions(result);
      } catch (error) {
        console.error(error);
      }
    }

    initializeRuntimeInstructions();
  }, []);

  useEffect(() => {
    if (!runtimeInstructions) {
      return;
    }

    try {
      const parsed = JSON.parse(runtimeInstructions);

      const objectiveData = parsed.Sections?.find(
        (section: any) => section.Id === "objectives"
      );

      setObjective(objectiveData ?? null);
    } catch (error) {
      console.error(error);
    }
  }, [runtimeInstructions]);

  return (
    <>
      <section className="objective-panel">
        <span>
          <h2>Current Objective</h2>
          <ul>
            {currentTask.map((condition, index) => (
              <li key={index}>{condition}</li>
            ))}
          </ul>
        </span>
        <span>
          <h2>Strategy</h2>
          <ul>
            {currentStrategy.map((condition, index) => (
              <li key={index}>{condition}</li>
            ))}
          </ul>
        </span>
        <span>
          <h2>Stop Conditions</h2>
          <ul>
            {currentEarlyReportConditions.map((condition, index) => (
              <li key={index}>{condition}</li>
            ))}
          </ul>
        </span>
      </section>
      <span>
        <p>**********</p>
        <pre>{JSON.stringify(objective, null, 2)}</pre>
        <p>**********</p>
      </span>
    </>
  )
}

export default Diablo2Main;
