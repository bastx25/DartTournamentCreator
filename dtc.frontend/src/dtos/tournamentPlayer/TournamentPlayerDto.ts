import z from "zod";
import { playerDtoSchema } from "../player/PlayerDto";

export const tournamentPlayerDtoSchema = z.object({
  id: z.number().int(),
  tournamentId: z.number().int(),
  playerId: z.number().int(),
  player: playerDtoSchema.nullable(),
});

export type TournamentPlayerDto = z.infer<typeof tournamentPlayerDtoSchema>;
