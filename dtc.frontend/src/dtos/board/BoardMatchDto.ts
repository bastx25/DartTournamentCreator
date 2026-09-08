import z from "zod";
import { matchParticipantDtoSchema } from "../MatchParticipant/MatchParticipantDto";
import { MatchStatus } from "../../enums/MatchStatus";

export const boardMatchDtoSchema = z.object({
  matchId: z.number(),
  roundId: z.number(),
  roundName: z.string().nullable(),
  tournamentName: z.string(),
  plannedStart: z.iso.datetime({ offset: true }).nullable(),
  actualStart: z.iso.datetime({ offset: true }).nullable(),
  status: z.enum(MatchStatus),
  participants: z.array(matchParticipantDtoSchema),
});

export type BoardMatchDto = z.infer<typeof boardMatchDtoSchema>;
