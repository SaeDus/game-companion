export interface ObjectiveRule {
    Id: string;
    Title: string;
    Content: string[];
}

export interface Objective {
    Id: string;
    Title: string;
    Rules: ObjectiveRule[];
}