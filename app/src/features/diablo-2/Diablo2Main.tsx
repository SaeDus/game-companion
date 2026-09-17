import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import { InstructionSection } from "./types/RuntimeInstructions.ts";

function Diablo2Main() {
  const [runtimeInstructions, setRuntimeInstructions] = useState("");
  const [instructionObjective, setInstructionObjective] = useState<InstructionSection | null>(null);
  const [instructionStopConditions, setInstructionStopConditions] = useState<InstructionSection | null>(null);
  const [instructionItemTracking, setInstructionItemTracking] = useState<InstructionSection | null>(null);
  const [instructionRunewordWatchlist, setInstructionRunewordWatchlist] = useState<InstructionSection | null>(null);

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

      const stopConditionData = parsed.Sections?.find(
        (section: any) => section.Id === "permanent-stopping-conditions"
      );

      const itemTrackingData = parsed.Sections?.find(
        (section: any) => section.Id === "manual-item-tracking"
      );

      const runewordWatchlistData = parsed.Sections?.find(
        (section: any) => section.Id === "runeword-base-watchlist"
      );

      setInstructionObjective(objectiveData ?? null);
      setInstructionStopConditions(stopConditionData ?? null);
      setInstructionItemTracking(itemTrackingData ?? null);
      setInstructionRunewordWatchlist(runewordWatchlistData ?? null);
    } catch (error) {
      console.error(error);
    }
  }, [runtimeInstructions]);

  return (
    <>
      <ObjectivePanel
        task={objectiveTask}
        strategy={objectiveStrategy}
        reportConditions={objectiveReportConditions}
      />
      <p>---</p>
      <StopConditionPanel stopConditions={instructionStopConditions} />
      <p>---</p>
      <ItemTrackingPanel items={instructionItemTracking} />
      <p>---</p>
      <RunewordWatchlistPanel watchlist={instructionRunewordWatchlist} />
    </>
  )
}

function ObjectivePanel({ task, strategy, reportConditions }: { task: string[], strategy: string[], reportConditions: string[] }) {
  return (
    <section className="objective-panel">
      <div>
        <h2>Current Objective</h2>
        <ul>
          {task.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </div>
      <div>
        <h2>Strategy</h2>
        <ul>
          {strategy.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </div>
      <div>
        <h2>Early Report Conditions</h2>
        <ul>
          {reportConditions.map((condition, index) => (
            <li key={index}>{condition}</li>
          ))}
        </ul>
      </div>
    </section>
  );
}

function StopConditionPanel({ stopConditions }: { stopConditions: InstructionSection | null }) {
  if (!stopConditions) {
    return <p>No Permanent Stop Conditions...</p>
  }

  return (
    <section className="stop-condition-panel">
      <h2>Permanent Stop Conditions</h2>
      <ul>
        {stopConditions.Rules.map((rule) => (
          <li key={rule.Id}>
            <h3>{rule.Title}</h3>

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

function ItemTrackingPanel({ items }: { items: InstructionSection | null }) {
  if (!items) {
    return <p>No Manual Item Tracking...</p>
  }

  return (
    <section className="item-tracking-panel">
      <h2>Manual Item Tracking</h2>
      <ul>
        {items.Rules.map((rule) => (
          <li key={rule.Id}>
            <h3>{rule.Title}</h3>

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

function RunewordWatchlistPanel({ watchlist }: { watchlist: InstructionSection | null }) {
  if (!watchlist) {
    return <p>No Runewords in Watchlist...</p>
  }

  return (
    <section className="runeword-watchlist-panel">
      <h2>Runeword Watchlist</h2>
      <ul>
        {watchlist.Rules.map((rule) => (
          <li key={rule.Id}>
            <h3>{rule.Title}</h3>

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
