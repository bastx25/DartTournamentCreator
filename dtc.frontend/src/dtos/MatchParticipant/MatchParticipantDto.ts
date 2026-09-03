import z from "zod";
import { playerDtoSchema } from "../Player/PlayerDto";

export const matchParticipantDtoSchema = z.object({
  id: z.number(),
  matchId: z.number(),
  player: playerDtoSchema.nullable(),
  score: z.number(),
  isWinner: z.boolean(),
});

export type MatchParticipantDto = z.infer<typeof matchParticipantDtoSchema>;
