import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import { InstructionSection } from "./types/RuntimeInstructions.ts";

function Diablo2Main() {
  const [runtimeInstructions, setRuntimeInstructions] = useState("");
  const [instructionObjective, setInstructionObjective] = useState<InstructionSection | null>(null);
  const [instructionStopConditions, setInstructionStopConditions] = useState<InstructionSection | null>(null);

  // Objective
  const objectiveTask =
    instructionObjective?.Rules.find(
      (rule) => rule.Id === "current-task"
    )?.Content ?? [];

  const objectiveStrategy =
    instructionObjective?.Rules.find(
      (rule) => rule.Id === "strategy"
    )?.Content ?? [];

  const objectiveReportConditions =
    instructionObjective?.Rules.find(
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

      setInstructionObjective(objectiveData ?? null);
    } catch (error) {
      console.error(error);
    }
  }, [runtimeInstructions]);

  useEffect(() => {
    if (!runtimeInstructions) {
      return;
    }

    try {
      const parsed = JSON.parse(runtimeInstructions);

      const stopConditionData = parsed.Sections?.find(
        (section: any) => section.Id === "permanent-stopping-conditions"
      );

      setInstructionStopConditions(stopConditionData ?? null);
    } catch (error) {
      console.error(error);
    }
  }, [runtimeInstructions]);

  return (
    <>
      <DrawObjectivePanel
        task={objectiveTask}
        strategy={objectiveStrategy}
        reportConditions={objectiveReportConditions}
      />
      <DrawStopConditionPanel stopConditions={instructionStopConditions} />
    </>
  )
}

function DrawObjectivePanel({ task, strategy, reportConditions }: { task: String[], strategy: String[], reportConditions: String[] }) {
  return (
    <section className="objective-panel">
      <span>
        <h2>Current Objective</h2>
        <ul>
          {task.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </span>
      <span>
        <h2>Strategy</h2>
        <ul>
          {strategy.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </span>
      <span>
        <h2>Stop Conditions</h2>
        <ul>
          {reportConditions.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </span>
    </section>
  );
}

function DrawStopConditionPanel({ stopConditions }: { stopConditions: InstructionSection | null }) {
  if (!stopConditions) {
    return <p>No Permanent Stop Conditions...</p>
  }

  return (
    <section className="stop-condition-panel">
      <ul>
        {stopConditions.Rules.map((rule) => (
          <li key={rule.Id}>
            <strong>{rule.Title}</strong>

            <ul>
              {rule.Content.map((ruleText, index) => (
                <li key={index}>{ruleText}</li>
              ))}
            </ul>
          </li>
        ))}
      </ul>
    </section>
  );
}

export default Diablo2Main;
