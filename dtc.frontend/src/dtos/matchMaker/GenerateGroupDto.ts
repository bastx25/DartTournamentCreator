import z from "zod";

export const generateGroupsDtoSchema = z.object({
  groupCount: z.number().int().min(1).max(64),
  playersPerGroup: z.number().int().min(1).max(64).nullable(),
  qualifiersPerGroup: z.number().int().min(1).max(64),
  startTime: z.iso.datetime({ offset: true }).nullable(),
  matchDurationMinutes: z.number().int().min(1).max(240).nullable(),
  breakBetweenMatchesMinutes: z.number().int().min(0).max(120).nullable(),
  playerIds: z.array(z.number().int()).nullable(),
});

export type GenerateGroupsDto = z.infer<typeof generateGroupsDtoSchema>;
