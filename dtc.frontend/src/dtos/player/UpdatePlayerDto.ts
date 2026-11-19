import { z } from "zod";

export const updatePlayerDtoSchema = z.object({
  firstName: z
    .string()
    .trim()
    .min(1, "Der Vorname ist ein Pflichtfeld.")
    .max(50, "Der Vorname darf maximal 50 Zeichen lang sein."),
  lastName: z
    .string()
    .trim()
    .max(50, "Der Nachname darf maximal 50 Zeichen lang sein.")
    .nullable()
    .optional(),
  nickname: z
    .string()
    .trim()
    .max(50, "Der Spitzname darf maximal 50 Zeichen lang sein.")
    .nullable()
    .optional(),
});

export type UpdatePlayerDto = z.infer<typeof updatePlayerDtoSchema>;
