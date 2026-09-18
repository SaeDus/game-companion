import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";
import {
  AlertTriangle,
  BookOpen,
  Gem,
  PackageSearch,
  ScrollText,
} from "lucide-react";

import {
  InstructionSection,
  RunewordSection,
} from "../types/RuntimeInstructions.ts";

import "./Diablo2Watchlist.css";

function Diablo2Watchlist() {
  const [instructionItemTracking, setInstructionItemTracking] =
    useState<InstructionSection | null>(null);
  const [instructionRunewordWatchlist, setInstructionRunewordWatchlist] =
    useState<RunewordSection | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

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

        const itemTrackingData = parsed.Sections?.find(
          (section: any) => section.Id === "manual-item-tracking"
        );

        const runewordWatchlistData = parsed.Sections?.find(
          (section: any) => section.Id === "runeword-base-watchlist"
        );

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
    <main className="d2-page d2-watchlist-page">
      <div className="d2-page-backdrop" aria-hidden="true" />

      <div className="d2-page-shell">
        <header className="d2-page-header">
          <div className="d2-page-heading">
            <div className="d2-eyebrow">
              <BookOpen size={15} aria-hidden="true" />
              Field Intelligence
            </div>

            <h1>
              Diablo II <span>Watchlist</span>
            </h1>

            <p>
              Loot priorities at a glance: tracked item categories, runeword
              targets, valid bases, and notes for the active run.
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

            <div className="d2-runtime-metrics" aria-label="Watchlist summary">
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

        <section className="d2-reference-grid" aria-label="Field intelligence">
          <ItemTrackingPanel items={instructionItemTracking} />
          <RunewordWatchlistPanel watchlist={instructionRunewordWatchlist} />
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
                        <span
                          className="d2-rune-chip"
                          key={runeword.Id + "-rune-" + index}
                        >
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
                        <li key={runeword.Id + "-base-" + index}>
                          {baseItem}
                        </li>
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
                        <p key={runeword.Id + "-note-" + index}>{note}</p>
                      ))}
                    </div>
                  ) : (
                    <span className="d2-muted-text">
                      No additional notes.
                    </span>
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

export default Diablo2Watchlist;
