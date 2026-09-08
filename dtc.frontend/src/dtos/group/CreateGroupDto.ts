import z from "zod";

export const createGroupDtoSchema = z.object({
  tournamentId: z.number().int().min(1),
  sequence: z.number().int().min(1),
  name: z.string().min(1).max(100),
  qualifiersCount: z.number().int().min(1),
});

export type CreateGroupDto = z.infer<typeof createGroupDtoSchema>;
