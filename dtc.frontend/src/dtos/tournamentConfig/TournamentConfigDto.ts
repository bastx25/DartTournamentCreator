import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";

export const tournamentConfigDtoSchema = z.object({
  id: z.number().int(),
  tournamentId: z.number().int(),
  versionNr: z.number().int(),
  mode: z.enum(TournamentMode),
  matchDurationMinutes: z.number().int(),
  breakBetweenMatchesMinutes: z.number().int(),
  groupCount: z.number().int(),
  playersPerGroup: z.number().int().nullable(),
  qualifiersPerGroup: z.number().int(),
});

export type TournamentConfigDto = z.infer<typeof tournamentConfigDtoSchema>;
