import z from "zod";

export const updateGroupPlayerDtoSchema = z.object({
  groupId: z.number().int().min(1),
  tournamentPlayerId: z.number().int().min(1),
});

export type UpdateGroupPlayerDto = z.infer<typeof updateGroupPlayerDtoSchema>;
