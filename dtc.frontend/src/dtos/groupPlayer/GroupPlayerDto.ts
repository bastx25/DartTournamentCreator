import z from "zod";

export const groupPlayerDtoSchema = z.object({
  id: z.number(),
  groupId: z.number(),
  tournamentPlayerId: z.number(),
});

export type GroupPlayerDto = z.infer<typeof groupPlayerDtoSchema>;
