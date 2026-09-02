import z from "zod";
import { MatchStatus } from "../../enums/MatchStatus";

export const updateMatchDtoSchema = z.object({
  boardId: z.number().nullable(),

  status: z.enum(MatchStatus),

  actualStart: z.string().datetime({ offset: true }).nullable(),

  actualEnd: z.string().datetime({ offset: true }).nullable(),
});

export type UpdateMatchDto = z.infer<typeof updateMatchDtoSchema>;
