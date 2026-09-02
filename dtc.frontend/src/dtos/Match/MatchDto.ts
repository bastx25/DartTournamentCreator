import z from "zod";
import { matchParticipantDtoSchema } from "../MatchParticipant/MatchParticipantDto";

export const matchDtoSchema = z.object({
  id: z.number(),
  roundId: z.number(),
  boardId: z.number().nullable(),

  status: z.enum(["Scheduled", "InProgress", "Completed", "Cancelled"]),

  actualStart: z.string().datetime({ offset: true }).nullable(),
  actualEnd: z.string().datetime({ offset: true }).nullable(),

  participants: z.array(matchParticipantDtoSchema),
});

export type MatchDto = z.infer<typeof matchDtoSchema>;
