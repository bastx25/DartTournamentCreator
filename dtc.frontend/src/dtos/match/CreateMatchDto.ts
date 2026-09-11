import z from "zod";
import { MatchStatus } from "../../enums/MatchStatus";

export const createMatchDtoSchema = z.object({
  braketId: z.number().int().min(1),
  groupId: z.number().int().nullable(),
  boardId: z.number().int().nullable(),
  status: z.enum(MatchStatus),
  plannedStart: z.iso.datetime({ offset: true }).nullable(),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  actualStart: z.iso.datetime({ offset: true }).nullable(),
  actualEnd: z.iso.datetime({ offset: true }).nullable(),
});

export type CreateMatchDto = z.infer<typeof createMatchDtoSchema>;
