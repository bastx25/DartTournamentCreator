import z from "zod";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const createTournamentDtoSchema = z.object({
  name: z.string().min(1).max(100),

  startDate: z.iso.datetime({ offset: true }),

  description: z.string().max(500).nullable(),

  mode: z.enum(TournamentMode).default(TournamentMode.GroupStage),

  status: z.enum(TournamentStatus).default(TournamentStatus.Draft),
});

export type CreateTournamentDto = z.infer<typeof createTournamentDtoSchema>;
