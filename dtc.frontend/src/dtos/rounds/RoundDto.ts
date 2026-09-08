import z from "zod";
import { RoundStatus } from "../../enums/RoundStatus";
import { RoundPhase } from "../../enums/RoundPhase";
import { matchDtoSchema } from "../match/MatchDto";

export const roundDtoSchema = z.object({
  id: z.number().int(),
  tournamentId: z.number().int(),
  sequence: z.number().int(),
  name: z.string().max(100).nullable(),
  plannedStart: z.iso.datetime({ offset: true }),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  status: z.enum(RoundStatus),
  phase: z.enum(RoundPhase),
  matches: z.array(matchDtoSchema),
});

export type RoundDto = z.infer<typeof roundDtoSchema>;
