import z from "zod";

export const updateMatchParticipantDtoSchema = z.object({
  score: z.number().min(0, "Der Score muss mindestens 0 sein."),

  isWinner: z.boolean(),
});

export type UpdateMatchParticipantDto = z.infer<
  typeof updateMatchParticipantDtoSchema
>;
