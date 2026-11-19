import z from "zod";

export const playerDtoSchema = z.object({
  id: z.number().int(),
  firstName: z.string(),
  lastName: z.string().nullable().optional(),
  nickname: z.string().nullable().optional(),
  displayName: z.string().optional(),
});

export type PlayerDto = z.infer<typeof playerDtoSchema>;
