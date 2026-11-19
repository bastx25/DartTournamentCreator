import type { TournamentStatus } from "../../enums/TournamentStatus";
import type { GroupDto } from "../group/GroupDto";
import type { BraketDto } from "../brakets/BraketDto";
import type { TournamentConfigDto } from "../tournamentConfig/TournamentConfigDto";

export interface PublicTournamentDto {
  id: number;
  name: string;
  code: string;
  description?: string;
  status: TournamentStatus;
  startDate: string;
  config?: TournamentConfigDto;
  groups: GroupDto[];
  brakets: BraketDto[];
}
