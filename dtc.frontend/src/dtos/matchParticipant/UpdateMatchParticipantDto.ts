import z from "zod";

export const updateMatchParticipantDtoSchema = z.object({
  matchId: z.number().int().min(1),
  tournamentPlayerId: z.number().int().min(1),
  score: z.number().int().min(0),
  isWinner: z.boolean(),
});

export type UpdateMatchParticipantDto = z.infer<
  typeof updateMatchParticipantDtoSchema
>;
