import z from "zod";
import { BraketStatus } from "../../enums/BraketStatus";
import { BraketPhase } from "../../enums/BraketPhase";

export const braketDtoSchema = z.object({
  id: z.number().int(),
  tournamentId: z.number().int(),
  sequence: z.number().int(),
  name: z.string().max(100).nullable(),
  plannedStart: z.iso.datetime({ offset: true }),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  status: z.enum(BraketStatus),
  phase: z.enum(BraketPhase),
});

export type BraketDto = z.infer<typeof braketDtoSchema>;
