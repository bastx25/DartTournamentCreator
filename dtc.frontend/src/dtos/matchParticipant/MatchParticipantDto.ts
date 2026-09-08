import z from "zod";

export const matchParticipantDtoSchema = z.object({
  id: z.number().int(),
  matchId: z.number().int(),
  tournamentPlayerId: z.number(),
  score: z.number().int(),
  isWinner: z.boolean(),
});

export type MatchParticipantDto = z.infer<typeof matchParticipantDtoSchema>;
