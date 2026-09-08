import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const tournamentDtoSchema = z.object({
  id: z.number().int(),
  name: z.string(),
  startDate: z.iso.datetime({ offset: true }),
  description: z.string().nullable(),
  mode: z.enum(TournamentMode),
  status: z.enum(TournamentStatus),
  matchDurationMinutes: z.number().int(),
  breakBetweenMatchesMinutes: z.number().int(),
});

export type TournamentDto = z.infer<typeof tournamentDtoSchema>;
