import z from "zod";

export const generateGroupsDtoSchema = z.object({
  groupCount: z.number().int().min(1).default(1),
  groupSize: z.number().int().min(2).default(4),
  qualifiersPerGroup: z.number().int().min(1).default(1),
  startTime: z.iso.datetime({ offset: true }).nullable(),
  matchDurationMinutes: z.number().int().min(1).nullable(),
  breakBetweenMatchesMinutes: z.number().int().min(0).nullable(),
  playerIds: z.array(z.number()).nullable(),
});

export type GenerateGroupsDto = z.infer<typeof generateGroupsDtoSchema>;
