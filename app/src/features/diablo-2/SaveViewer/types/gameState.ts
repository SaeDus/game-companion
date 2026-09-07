export interface Attributes {
  Strength: number;
  Dexterity: number;
  Vitality: number;
  Energy: number;
}

export interface Skill {
  Id?: number;
  Name: string;
  Level?: number;
}

export interface Item {
  ItemSeed?: number;
  Slot?: string;
  Name?: string;
  BaseName: string;
  BaseCode: string;
  ItemLevel?: number;
  Quality?: string;
  Defense?: number;
  Quantity?: number;
  Stats?: string[];
}

export interface Character {
  Name: string;
  Level: number;
  Class: string;
  Attributes: Attributes;
  UnspentStatPoints: number;
  Skills: Skill[];
  UnspentSkillPoints: number;
  Equipment: Item[];
  Inventory: Item[];
}

export interface Mercenary {
  Id: number;
  NameIndex: number;
  Hireling: string;
  SubType: string;
  Level: number;
  Experience: number;
  Strength: number;
  Dexterity: number;
  Skills: string[];
  Equipment: Item[];
}

export type QuestState =
  | "NotStarted"
  | "InProgress"
  | "Completed";

export interface Quest {
  Name: string;
  IsOptional: boolean;
  State: QuestState;
}

export interface Act {
  Quests: Quest[];
}

export interface Difficulty {
  ActI: Act;
  ActII: Act;
  ActIII: Act;
  ActIV: Act;
  ActV: Act;
}

export interface QuestLog {
  Normal: Difficulty;
  Nightmare: Difficulty;
  Hell: Difficulty;
}

export interface GameState {
  SchemaVersion: number;
  GeneratedAt: string;
  CharacterSaveModifiedAt: string;
  StashSaveModifiedAt: string;
  Character: Character;
  Mercenary: Mercenary;
  Stash: Item[];
  QuestLog: QuestLog;
}
