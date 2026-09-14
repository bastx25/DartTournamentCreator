import z from "zod";
import { TournamentStatus } from "../../enums/TournamentStatus";
import { TournamentMode } from "../../enums/TournamentMode";

export const createTournamentDtoSchema = z.object({
  name: z.string().min(1).max(100),
  description: z.string().max(1000).nullable(),
  mode: z.enum(TournamentMode),
  status: z.enum(TournamentStatus),
  startDate: z.string(),
});

export type CreateTournamentDto = z.infer<typeof createTournamentDtoSchema>;
