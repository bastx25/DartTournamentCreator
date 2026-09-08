import z from "zod";

export const groupDtoSchema = z.object({
  id: z.number(),
  tournamentId: z.number(),
  sequence: z.number(),
  name: z.string(),
  qualifiersCount: z.number(),
});

export type GroupDto = z.infer<typeof groupDtoSchema>;
