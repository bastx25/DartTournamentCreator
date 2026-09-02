import z from "zod";

export const generateGroupsDtoSchema = z.object({
  groupSize: z.number().default(4),
  playerIds: z.array(z.number()).nullable(),
});

export type GenerateGroupsDto = z.infer<typeof generateGroupsDtoSchema>;
