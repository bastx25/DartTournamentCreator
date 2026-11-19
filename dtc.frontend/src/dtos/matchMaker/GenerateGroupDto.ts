import z from "zod";
import { KnockoutDrawMode } from "../../enums/KnockoutDrawMode";

export const generateGroupsDtoSchema = z.object({
  groupCount: z.number().int().min(1).max(64),
  playersPerGroup: z.number().int().min(1).max(64).nullable(),
  qualifiersPerGroup: z.number().int().min(1).max(64),
  startTime: z.iso.datetime({ offset: true }).nullable(),
  matchDurationMinutes: z.number().int().min(1).max(240).nullable(),
  breakBetweenMatchesMatchesMinutes: z.number().int().min(0).max(120).nullable().optional(),
  breakBetweenMatchesMinutes: z.number().int().min(0).max(120).nullable(),
  playerIds: z.array(z.number().int()).nullable(),
  knockoutDrawMode: z.nativeEnum(KnockoutDrawMode).optional(),
});

export type GenerateGroupsDto = z.infer<typeof generateGroupsDtoSchema>;
