import z from "zod";
import { BraketStatus } from "../../enums/BraketStatus";
import { BraketPhase } from "../../enums/BraketPhase";

export const createBraketDtoSchema = z.object({
  tournamentId: z.number().int().min(1),
  sequence: z.number().int().min(1),
  name: z.string().max(100).nullable(),
  plannedStart: z.iso.datetime({ offset: true }),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),
  status: z.enum(BraketStatus),
  phase: z.enum(BraketPhase),
});

export type CreateBraketDto = z.infer<typeof createBraketDtoSchema>;
