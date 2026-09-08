import z from "zod";
import { tournamentPlayerDtoSchema } from "../tournamentPlayer/TournamentPlayer";

export const matchParticipantDtoSchema = z.object({
  id: z.number().int(),
  matchId: z.number().int(),
  tournamentPlayerId: z.number().int(),
  tournamentPlayer: tournamentPlayerDtoSchema.nullable(),
  score: z.number().int(),
  isWinner: z.boolean(),
});

export type MatchParticipantDto = z.infer<typeof matchParticipantDtoSchema>;
