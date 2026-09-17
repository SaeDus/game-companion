import { useState } from "react";
import {
  Database,
  LayoutDashboard,
  Menu,
  type LucideIcon,
} from "lucide-react";

import Diablo2Main from "./Diablo2Main";
import Diablo2Exporter from "./Diablo2Exporter";

import "./styles/Diablo2.css";
import "./styles/Diablo2Main.css";
import "./styles/Diablo2StateBuilder.css";

type Diablo2PageId = "main" | "exporter";

type Diablo2NavigationItem = {
  Id: Diablo2PageId;
  Label: string;
  Icon: LucideIcon;
};

const navigationItems: Diablo2NavigationItem[] = [
  {
    Id: "main",
    Label: "Command Hub",
    Icon: LayoutDashboard,
  },
  {
    Id: "exporter",
    Label: "State Exporter",
    Icon: Database,
  },
];

function Diablo2StateBuilder() {
  const [activePage, setActivePage] = useState<Diablo2PageId>("main");
  const [navigationExpanded, setNavigationExpanded] = useState(false);

  function renderActivePage() {
    switch (activePage) {
      case "exporter":
        return <Diablo2Exporter />;

      case "main":
      default:
        return <Diablo2Main />;
    }
  }

  return (
    <div className="diablo2-theme d2-state-shell">
      <aside
        className={`d2-navigation ${navigationExpanded ? "is-expanded" : ""}`}
        aria-label="Diablo II navigation"
      >
        <div className="d2-navigation-header">
          <button
            type="button"
            className="d2-navigation-toggle"
            onClick={() => setNavigationExpanded((expanded) => !expanded)}
            aria-label={navigationExpanded ? "Collapse navigation" : "Expand navigation"}
            aria-expanded={navigationExpanded}
            title={navigationExpanded ? "Collapse navigation" : "Expand navigation"}
          >
            <Menu size={20} aria-hidden="true" />
          </button>

          <div className="d2-navigation-brand" aria-hidden={!navigationExpanded}>
            <span>Diablo II</span>
            <small>Game Companion</small>
          </div>
        </div>

        <nav className="d2-navigation-list">
          {navigationItems.map((item) => {
            const Icon = item.Icon;
            const isActive = activePage === item.Id;

            return (
              <button
                type="button"
                className={`d2-navigation-item ${isActive ? "is-active" : ""}`}
                key={item.Id}
                onClick={() => setActivePage(item.Id)}
                aria-current={isActive ? "page" : undefined}
                aria-label={item.Label}
                title={!navigationExpanded ? item.Label : undefined}
              >
                <span className="d2-navigation-icon">
                  <Icon size={19} aria-hidden="true" />
                </span>

                <span className="d2-navigation-label">{item.Label}</span>
              </button>
            );
          })}
        </nav>

        <div className="d2-navigation-footer" aria-hidden={!navigationExpanded}>
          <span>Reign of the Warlock</span>
        </div>
      </aside>

      <div className="d2-page-viewport">{renderActivePage()}</div>
    </div>
  );
}

export default Diablo2StateBuilder;
