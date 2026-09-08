import z from "zod";
import { RoundStatus } from "../../enums/RoundStatus";
import { RoundPhase } from "../../enums/RoundPhase";

export const createRoundDtoSchema = z.object({
  tournamentId: z.number().int().min(1),
  sequence: z.number().int().min(1),
  name: z.string().max(100).nullable(),
  plannedStart: z.iso.datetime({ offset: true }),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  status: z.enum(RoundStatus),
  phase: z.enum(RoundPhase),
});

export type CreateRoundDto = z.infer<typeof createRoundDtoSchema>;
