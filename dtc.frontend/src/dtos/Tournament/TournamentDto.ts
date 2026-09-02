import z from "zod";
import { roundDtoSchema } from "../Round/RoundDto";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";

export const tournamentDtoSchema = z.object({
  id: z.number(),
  name: z.string(),
  startDate: z.iso.datetime({ offset: true }),
  description: z.string().nullable(),
  mode: z.enum(TournamentMode),
  status: z.enum(TournamentStatus),
  rounds: z.array(roundDtoSchema),
});

export type TournamentDto = z.infer<typeof tournamentDtoSchema>;
