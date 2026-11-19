import { KnockoutDrawMode } from "../../enums/KnockoutDrawMode";

export interface ManualGroupItemDto {
  name: string;
  playerIds: number[];
  qualifiersCount: number;
}

export interface ManualGroupSetupDto {
  groups: ManualGroupItemDto[];
  startTime?: string;
  matchDurationMinutes?: number;
  breakBetweenMatchesMinutes?: number;
  knockoutDrawMode?: KnockoutDrawMode;
}
