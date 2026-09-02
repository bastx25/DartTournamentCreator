import z from "zod";

export const playerDtoSchema = z.object({
  id: z.number(),
  firstName: z.string(),
  lastName: z.string(),
  nickname: z.string().nullable(),
  displayName: z.string(),
});

export type PlayerDto = z.infer<typeof playerDtoSchema>;
