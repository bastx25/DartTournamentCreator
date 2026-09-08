import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";
import { groupDtoSchema } from "../group/GroupDto";
import { roundDtoSchema } from "../rounds/RoundDto";
import { tournamentPlayerDtoSchema } from "../tournamentPlayer/TournamentPlayer";

export const tournamentDtoSchema = z.object({
  id: z.number().int(),
  name: z.string(),
  startDate: z.iso.datetime({ offset: true }),
  description: z.string().nullable(),
  mode: z.enum(TournamentMode),
  status: z.enum(TournamentStatus),
  matchDurationMinutes: z.number().int(),
  breakBetweenMatchesMinutes: z.number().int(),
  tournamentPlayers: z.array(tournamentPlayerDtoSchema),
  groups: z.array(groupDtoSchema).nullable(),
  rounds: z.array(roundDtoSchema).nullable(),
});

export type TournamentDto = z.infer<typeof tournamentDtoSchema>;
