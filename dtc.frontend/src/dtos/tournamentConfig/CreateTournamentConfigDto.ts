import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";

export const createTournamentConfigDtoSchema = z.object({
  tournamentId: z.number().int(),
  versionNr: z.number().int(),
  mode: z.enum(TournamentMode),
  matchDurationMinutes: z.number().int().min(1),
  breakBetweenMatchesMinutes: z.number().int().min(0),
  groupCount: z.number().int().min(1),
  playersPerGroup: z.number().int().min(1).nullable(),
  qualifiersPerGroup: z.number().int().min(1),
});

export type CreateTournamentConfigDto = z.infer<
  typeof createTournamentConfigDtoSchema
>;
