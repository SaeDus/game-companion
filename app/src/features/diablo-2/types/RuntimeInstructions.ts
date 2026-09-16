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
