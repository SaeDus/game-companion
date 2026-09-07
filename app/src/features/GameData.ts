export interface GameContent {
  Id: string;
  Title: string;
  Type: string;
}

export interface GameData {
  Id: string;
  Title: string;
  Content: GameContent;
}
