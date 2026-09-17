export interface InstructionSection {
  Id: string;
  Title: string;
  Rules: InstructionRule[];
}

export interface InstructionRule {
  Id: string;
  Title: string;
  Content: string[];
}

export interface RunewordSection {
  Id: string;
  Title: string;
  Runewords: RunewordItem[];
}

export interface RunewordItem {
  Id: string;
  Name: string;
  Runes: string[];
  BaseItems: string[];
  Notes: string[];
}
