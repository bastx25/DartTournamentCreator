import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const updateTournamentDtoSchema = z.object({
  name: z.string().min(1).max(100),

  startDate: z.iso.datetime({ offset: true }),

  description: z.string().max(500).nullable(),

  mode: z.enum(TournamentMode),

  status: z.enum(TournamentStatus),
});

export type UpdateTournamentDto = z.infer<typeof updateTournamentDtoSchema>;
