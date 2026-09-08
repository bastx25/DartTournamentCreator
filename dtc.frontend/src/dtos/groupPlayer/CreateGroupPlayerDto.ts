import z from "zod";

export const createGroupPlayerDtoSchema = z.object({
  groupId: z.number().int().min(1),
  tournamentPlayerId: z.number().int().min(1),
});

export type CreateGroupPlayerDto = z.infer<typeof createGroupPlayerDtoSchema>;
