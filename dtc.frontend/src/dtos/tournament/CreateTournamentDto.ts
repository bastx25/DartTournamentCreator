import z from "zod";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const createTournamentDtoSchema = z.object({
  name: z.string().min(1).max(100),
  description: z.string().max(1000).nullable(),
  status: z.enum(TournamentStatus),
  startDate: z.string(),
});

export type CreateTournamentDto = z.infer<typeof createTournamentDtoSchema>;
