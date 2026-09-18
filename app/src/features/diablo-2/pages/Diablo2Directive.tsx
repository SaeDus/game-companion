import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import {
  AlertTriangle,
  Compass,
  Route,
  ShieldAlert,
  Sparkles,
  Target,
} from "lucide-react";
import { InstructionSection } from "../types/RuntimeInstructions.ts";

import "./Diablo2Directive.css";

function Diablo2Directive() {
  const [instructionObjective, setInstructionObjective] =
    useState<InstructionSection | null>(null);
  const [instructionStopConditions, setInstructionStopConditions] =
    useState<InstructionSection | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

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
        setLoadError(null);
        setIsLoading(true);

        const result = await invoke<string>(
          "get_runtime_instructions",
          {
            characterId: "vaelric",
          }
        );

        const parsed = JSON.parse(result);

        const objectiveData = parsed.Sections?.find(
          (section: any) => section.Id === "objectives"
        );

        const stopConditionData = parsed.Sections?.find(
          (section: any) => section.Id === "permanent-stopping-conditions"
        );

        setInstructionObjective(objectiveData ?? null);
        setInstructionStopConditions(stopConditionData ?? null);
      } catch (error) {
        console.error(error);
        setLoadError(
          error instanceof Error
            ? error.message
            : "Unable to load runtime instructions."
        );
      } finally {
        setIsLoading(false);
      }
    }

    initializeRuntimeInstructions();
  }, []);

  const runtimeStatus = loadError
    ? "Runtime unavailable"
    : isLoading
      ? "Loading runtime"
      : "Runtime linked";

  return (
    <main className="d2-page d2-directive-page">
      <div className="d2-page-backdrop" aria-hidden="true" />

      <div className="d2-page-shell">
        <header className="d2-page-header">
          <div className="d2-page-heading">
            <div className="d2-eyebrow">
              <Compass size={15} aria-hidden="true" />
              Live Run Directive
            </div>

            <h1>
              Diablo II <span>Directive</span>
            </h1>

            <p>
              Keep the current objective, strategy, report conditions, and hard
              stops visible while the run is in motion.
            </p>
          </div>

          <div className="d2-page-summary">
            <div
              className={
                "d2-runtime-status " +
                (loadError ? "is-error" : isLoading ? "is-loading" : "is-ready")
              }
            >
              <span aria-hidden="true" />
              {runtimeStatus}
            </div>

            <div className="d2-runtime-metrics" aria-label="Directive summary">
              <RuntimeMetric
                label="Stop rules"
                value={instructionStopConditions?.Rules.length ?? 0}
              />
              <RuntimeMetric
                label="Early reports"
                value={objectiveReportConditions.length}
              />
            </div>
          </div>
        </header>

        {loadError && (
          <div className="d2-load-error" role="alert">
            <AlertTriangle size={18} aria-hidden="true" />
            <div>
              <strong>Runtime instructions could not be loaded.</strong>
              <span>{loadError}</span>
            </div>
          </div>
        )}

        <section className="d2-primary-grid" aria-label="Run directive">
          <ObjectivePanel
            task={objectiveTask}
            strategy={objectiveStrategy}
            reportConditions={objectiveReportConditions}
          />

          <StopConditionPanel stopConditions={instructionStopConditions} />
        </section>
      </div>
    </main>
  );
}

function RuntimeMetric({ label, value }: { label: string; value: number }) {
  return (
    <div className="d2-runtime-metric">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

function ObjectivePanel({
  task,
  strategy,
  reportConditions,
}: {
  task: string[];
  strategy: string[];
  reportConditions: string[];
}) {
  return (
    <article className="d2-panel d2-objective-panel">
      <div className="d2-panel-header d2-objective-header">
        <div className="d2-panel-icon d2-panel-icon-gold">
          <Target size={21} aria-hidden="true" />
        </div>

        <div>
          <span className="d2-panel-kicker">Primary Directive</span>
          <h2>Current Objective</h2>
        </div>
      </div>

      <div className="d2-current-task">
        <span className="d2-field-label">Current Task</span>

        {task.length > 0 ? (
          <div className="d2-task-lines">
            {task.map((taskLine, index) => (
              <p key={index}>{taskLine}</p>
            ))}
          </div>
        ) : (
          <EmptyMessage message="No current task is available." />
        )}
      </div>

      <div className="d2-objective-support-grid">
        <section className="d2-objective-section">
          <div className="d2-subsection-heading">
            <Route size={17} aria-hidden="true" />
            <h3>Strategy</h3>
          </div>

          {strategy.length > 0 ? (
            <div className="d2-copy-stack">
              {strategy.map((strategyLine, index) => (
                <p key={index}>{strategyLine}</p>
              ))}
            </div>
          ) : (
            <EmptyMessage message="No strategy has been provided." />
          )}
        </section>

        <section className="d2-objective-section d2-report-section">
          <div className="d2-subsection-heading">
            <Sparkles size={17} aria-hidden="true" />
            <h3>Early Report Conditions</h3>
          </div>

          {reportConditions.length > 0 ? (
            <ul className="d2-instruction-list">
              {reportConditions.map((condition, index) => (
                <li key={index}>{condition}</li>
              ))}
            </ul>
          ) : (
            <EmptyMessage message="No early report conditions are active." />
          )}
        </section>
      </div>
    </article>
  );
}

function StopConditionPanel({
  stopConditions,
}: {
  stopConditions: InstructionSection | null;
}) {
  return (
    <aside className="d2-panel d2-stop-panel">
      <div className="d2-panel-header">
        <div className="d2-panel-icon d2-panel-icon-red">
          <ShieldAlert size={20} aria-hidden="true" />
        </div>

        <div>
          <span className="d2-panel-kicker">Hard Overrides</span>
          <h2>Permanent Stop Conditions</h2>
        </div>
      </div>

      {!stopConditions || stopConditions.Rules.length === 0 ? (
        <EmptyMessage message="No permanent stop conditions are active." />
      ) : (
        <div className="d2-stop-list">
          {stopConditions.Rules.map((rule, index) => (
            <article className="d2-stop-card" key={rule.Id}>
              <div className="d2-stop-index">
                {String(index + 1).padStart(2, "0")}
              </div>

              <div>
                <h3>{rule.Title}</h3>
                <div className="d2-copy-stack">
                  {rule.Content.map((ruleText, contentIndex) => (
                    <p key={contentIndex}>{ruleText}</p>
                  ))}
                </div>
              </div>
            </article>
          ))}
        </div>
      )}
    </aside>
  );
}

function EmptyMessage({ message }: { message: string }) {
  return <p className="d2-empty-message">{message}</p>;
}

export default Diablo2Directive;
