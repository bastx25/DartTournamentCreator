import z from "zod";

export const createMatchParticipantDtoSchema = z.object({
  matchId: z.number().int().min(1),
  tournamentPlayerId: z.number().int().min(1),
  score: z.number().int().min(0),
  isWinner: z.boolean(),
});

export type CreateMatchParticipantDto = z.infer<
  typeof createMatchParticipantDtoSchema
>;
