import z from "zod";
import { MatchStatus } from "../../enums/MatchStatus";

export const createMatchDtoSchema = z.object({
  roundId: z.number({
    message: "RoundId ist ein Pflichtfeld.",
  }),

  boardId: z.number().nullable(),

  status: z.enum(MatchStatus).default(MatchStatus.Scheduled),
});

export type CreateMatchDto = z.infer<typeof createMatchDtoSchema>;
