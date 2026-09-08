import z from "zod";
import { tournamentPlayerDtoSchema } from "../tournamentPlayer/TournamentPlayer";

export const groupPlayerDtoSchema = z.object({
  id: z.number(),
  groupId: z.number(),
  tournamentPlayer: tournamentPlayerDtoSchema.nullable(),
});

export type GroupPlayerDto = z.infer<typeof groupPlayerDtoSchema>;
