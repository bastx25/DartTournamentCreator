import z from "zod";

export const boardParticipantScoreDtoSchema = z.object({
  participantId: z.number(),
  score: z.number().int().min(0),
});

export type BoardParticipantScoreDto = z.infer<
  typeof boardParticipantScoreDtoSchema
>;

export const finishBoardMatchDtoSchema = z.object({
  participants: z.array(boardParticipantScoreDtoSchema),
});

export type FinishBoardMatchDto = z.infer<typeof finishBoardMatchDtoSchema>;
