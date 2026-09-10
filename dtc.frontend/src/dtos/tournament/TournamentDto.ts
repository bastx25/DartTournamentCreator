import z from "zod";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const tournamentDtoSchema = z.object({
  id: z.number().int(),
  name: z.string(),
  description: z.string().nullable(),
  status: z.enum(TournamentStatus),
  startDate: z.string(),
});

export type TournamentDto = z.infer<typeof tournamentDtoSchema>;
