import z from "zod";
import { matchDtoSchema } from "../Match/MatchDto";

export const roundDtoSchema = z.object({
  id: z.number(),
  tournamentId: z.number(),
  locationId: z.number(),
  sequence: z.number(),

  name: z.string().nullable(),

  plannedStart: z.string().datetime({ offset: true }),
  plannedEnd: z.string().datetime({ offset: true }).nullable(),

  status: z.enum(["Scheduled", "InProgress", "Completed", "Cancelled"]),

  matches: z.array(matchDtoSchema),
});

export type RoundDto = z.infer<typeof roundDtoSchema>;
