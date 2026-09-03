import z from "zod";
import { MatchStatus } from "../../enums/MatchStatus";

export const createMatchDtoSchema = z.object({
  roundId: z.number({
    message: "RoundId ist ein Pflichtfeld.",
  }),

  groupId: z.number().nullable(),

  boardId: z.number().nullable(),

  status: z.enum(MatchStatus).default(MatchStatus.Scheduled),
  plannedStart: z.iso.datetime({ offset: true }).nullable(),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
});

export type CreateMatchDto = z.infer<typeof createMatchDtoSchema>;
