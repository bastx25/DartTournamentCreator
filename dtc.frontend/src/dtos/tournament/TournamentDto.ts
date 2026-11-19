import z from "zod";
import { TournamentStatus } from "../../enums/TournamentStatus";
import { TournamentMode } from "../../enums/TournamentMode";
import type { BraketDto } from "../brakets/BraketDto";

export const tournamentDtoSchema = z.object({
  id: z.number().int(),
  name: z.string(),
  code: z.string().optional(),
  description: z.string().nullable(),
  status: z.nativeEnum(TournamentStatus),
  startDate: z.string(),
  mode: z.nativeEnum(TournamentMode).optional(),
  userId: z.string().nullable().optional(),
});

export type TournamentDto = z.infer<typeof tournamentDtoSchema> & {
  brakets?: BraketDto[];
};
