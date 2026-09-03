import z from "zod";
import { matchParticipantDtoSchema } from "../MatchParticipant/MatchParticipantDto";
import { MatchStatus } from "../../enums/MatchStatus";

export const matchDtoSchema = z.object({
  id: z.number(),
  roundId: z.number(),
  groupId: z.number().nullable(),
  boardId: z.number().nullable(),

  status: z.enum(MatchStatus),

  plannedStart: z.iso.datetime({ offset: true }).nullable(),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),

  actualStart: z.iso.datetime({ offset: true }).nullable(),
  actualEnd: z.iso.datetime({ offset: true }).nullable(),

  participants: z.array(matchParticipantDtoSchema),
});

export type MatchDto = z.infer<typeof matchDtoSchema>;
