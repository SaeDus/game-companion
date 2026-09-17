import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import {
  AlertTriangle,
  BookOpen,
  Compass,
  Gem,
  PackageSearch,
  Route,
  ScrollText,
  ShieldAlert,
  Sparkles,
  Target,
} from "lucide-react";
import {
  InstructionSection,
  RunewordSection,
} from "./types/RuntimeInstructions.ts";

import "./styles/Diablo2Home.css";

function Diablo2Home() {
  const [instructionObjective, setInstructionObjective] =
    useState<InstructionSection | null>(null);
  const [instructionStopConditions, setInstructionStopConditions] =
    useState<InstructionSection | null>(null);
  const [instructionItemTracking, setInstructionItemTracking] =
    useState<InstructionSection | null>(null);
  const [instructionRunewordWatchlist, setInstructionRunewordWatchlist] =
    useState<RunewordSection | null>(null);
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
    <main className="d2-command-hub">
      <div className="d2-command-backdrop" aria-hidden="true" />

      <div className="d2-command-shell">
        <header className="d2-command-header">
          <div className="d2-command-heading">
            <div className="d2-eyebrow">
              <Compass size={15} aria-hidden="true" />
              Live Run Command
            </div>

            <h1>
              Diablo II <span>Command Hub</span>
            </h1>

            <p>
              Keep the current directive, stop rules, and loot priorities
              visible while the run is in motion.
            </p>
          </div>

          <div className="d2-command-summary">
            <div
              className={`d2-runtime-status ${
                loadError ? "is-error" : isLoading ? "is-loading" : "is-ready"
              }`}
            >
              <span aria-hidden="true" />
              {runtimeStatus}
            </div>

            <div className="d2-runtime-metrics" aria-label="Runtime summary">
              <RuntimeMetric
                label="Stop rules"
                value={instructionStopConditions?.Rules.length ?? 0}
              />
              <RuntimeMetric
                label="Tracked items"
                value={instructionItemTracking?.Rules.length ?? 0}
              />
              <RuntimeMetric
                label="Runewords"
                value={instructionRunewordWatchlist?.Runewords.length ?? 0}
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

        <section className="d2-primary-grid" aria-label="Run command">
          <ObjectivePanel
            task={objectiveTask}
            strategy={objectiveStrategy}
            reportConditions={objectiveReportConditions}
          />

          <StopConditionPanel stopConditions={instructionStopConditions} />
        </section>

        <section className="d2-reference-board" aria-labelledby="field-intelligence-heading">
          <div className="d2-reference-heading">
            <div>
              <div className="d2-eyebrow">
                <BookOpen size={14} aria-hidden="true" />
                Field Intelligence
              </div>
              <h2 id="field-intelligence-heading">Loot priorities at a glance</h2>
            </div>

            <p>
              Persistent reference material for the things worth slowing down
              to inspect during the run.
            </p>
          </div>

          <div className="d2-reference-grid">
            <ItemTrackingPanel items={instructionItemTracking} />
            <RunewordWatchlistPanel watchlist={instructionRunewordWatchlist} />
          </div>
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
              <div className="d2-stop-index">{String(index + 1).padStart(2, "0")}</div>

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

function ItemTrackingPanel({
  items,
}: {
  items: InstructionSection | null;
}) {
  return (
    <section className="d2-panel d2-watch-panel d2-item-panel">
      <div className="d2-panel-header">
        <div className="d2-panel-icon d2-panel-icon-violet">
          <PackageSearch size={20} aria-hidden="true" />
        </div>

        <div>
          <span className="d2-panel-kicker">Manual Review</span>
          <h2>Item Tracking</h2>
        </div>
      </div>

      {!items || items.Rules.length === 0 ? (
        <EmptyMessage message="No item categories are currently being tracked." />
      ) : (
        <div className="d2-item-grid">
          {items.Rules.map((rule) => (
            <article className="d2-item-card" key={rule.Id}>
              <div className="d2-card-title-row">
                <PackageSearch size={16} aria-hidden="true" />
                <h3>{rule.Title}</h3>
              </div>

              <div className="d2-copy-stack">
                {rule.Content.map((ruleText, index) => (
                  <p key={index}>{ruleText}</p>
                ))}
              </div>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}

function RunewordWatchlistPanel({
  watchlist,
}: {
  watchlist: RunewordSection | null;
}) {
  return (
    <section className="d2-panel d2-watch-panel d2-runeword-panel">
      <div className="d2-panel-header">
        <div className="d2-panel-icon d2-panel-icon-gold">
          <ScrollText size={20} aria-hidden="true" />
        </div>

        <div>
          <span className="d2-panel-kicker">Craft Targets</span>
          <h2>Runeword Watchlist</h2>
        </div>
      </div>

      {!watchlist || watchlist.Runewords.length === 0 ? (
        <EmptyMessage message="No runewords are currently being watched." />
      ) : (
        <div className="d2-runeword-list">
          {watchlist.Runewords.map((runeword) => (
            <article className="d2-runeword-card" key={runeword.Id}>
              <div className="d2-runeword-title">
                <div>
                  <span className="d2-card-kicker">Runeword</span>
                  <h3>{runeword.Name}</h3>
                </div>
                <Gem size={19} aria-hidden="true" />
              </div>

              <div className="d2-runeword-details">
                <div className="d2-runeword-field">
                  <span className="d2-field-label">Runes</span>
                  {runeword.Runes.length > 0 ? (
                    <div className="d2-rune-row">
                      {runeword.Runes.map((rune, index) => (
                        <span className="d2-rune-chip" key={`${runeword.Id}-rune-${index}`}>
                          {rune}
                        </span>
                      ))}
                    </div>
                  ) : (
                    <span className="d2-muted-text">Not specified</span>
                  )}
                </div>

                <div className="d2-runeword-field">
                  <span className="d2-field-label">Valid Bases</span>
                  {runeword.BaseItems.length > 0 ? (
                    <ul className="d2-compact-list">
                      {runeword.BaseItems.map((baseItem, index) => (
                        <li key={`${runeword.Id}-base-${index}`}>{baseItem}</li>
                      ))}
                    </ul>
                  ) : (
                    <span className="d2-muted-text">Not specified</span>
                  )}
                </div>

                <div className="d2-runeword-field d2-runeword-notes">
                  <span className="d2-field-label">Notes</span>
                  {runeword.Notes.length > 0 ? (
                    <div className="d2-copy-stack">
                      {runeword.Notes.map((note, index) => (
                        <p key={`${runeword.Id}-note-${index}`}>{note}</p>
                      ))}
                    </div>
                  ) : (
                    <span className="d2-muted-text">No additional notes.</span>
                  )}
                </div>
              </div>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}

function EmptyMessage({ message }: { message: string }) {
  return <p className="d2-empty-message">{message}</p>;
}

export default Diablo2Home;
