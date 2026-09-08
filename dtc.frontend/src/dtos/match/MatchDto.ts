import z from "zod";
import { MatchStatus } from "../../enums/MatchStatus";
import { matchParticipantDtoSchema } from "../matchParticipant/MatchParticipantDto";

export const matchDtoSchema = z.object({
  id: z.number(),
  roundId: z.number(),
  groupId: z.number().int().nullable(),
  boardId: z.number().int().nullable(),
  status: z.enum(MatchStatus),
  plannedStart: z.iso.datetime({ offset: true }).nullable(),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  actualStart: z.iso.datetime({ offset: true }).nullable(),
  actualEnd: z.iso.datetime({ offset: true }).nullable(),
  participants: z.array(matchParticipantDtoSchema),
});

export type MatchDto = z.infer<typeof matchDtoSchema>;
