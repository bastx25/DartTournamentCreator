import { z } from "zod";

export const updatePlayerDtoSchema = z.object({
  firstName: z.string().min(1).max(50),
  lastName: z.string().min(1).max(50),
  nickname: z.string().max(50).nullable(),
});

export type UpdatePlayerDto = z.infer<typeof updatePlayerDtoSchema>;
