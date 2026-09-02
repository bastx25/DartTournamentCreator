import z from "zod";

export const createMatchParticipantDtoSchema = z.object({
  matchId: z.number({
    message: "MatchId ist ein Pflichtfeld.",
  }),

  playerId: z.number({
    message: "PlayerId ist ein Pflichtfeld.",
  }),

  score: z.number().min(0, "Der Score muss mindestens 0 sein.").default(0),

  isWinner: z.boolean().default(false),
});

export type CreateMatchParticipantDto = z.infer<
  typeof createMatchParticipantDtoSchema
>;
