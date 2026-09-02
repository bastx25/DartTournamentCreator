import z from "zod";
import { matchDtoSchema } from "../Match/MatchDto";
import { RoundStatus } from "../../enums/RoundStatus";

export const roundDtoSchema = z.object({
  id: z.number(),
  tournamentId: z.number(),
  locationId: z.number(),
  sequence: z.number(),

  name: z.string().nullable(),

  plannedStart: z.iso.datetime({ offset: true }),
  plannedEnd: z.iso.datetime({ offset: true }).nullable(),

  status: z.enum(RoundStatus),

  matches: z.array(matchDtoSchema),
});

export type RoundDto = z.infer<typeof roundDtoSchema>;
