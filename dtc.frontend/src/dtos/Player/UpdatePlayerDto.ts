import z from "zod";

export const updatePlayerDtoSchema = z.object({
  firstName: z
    .string({
      message: "Der Vorname ist ein Pflichtfeld.",
    })
    .min(1, "Der Vorname ist ein Pflichtfeld.")
    .max(50, "Der Vorname darf maximal 50 Zeichen lang sein."),

  lastName: z
    .string({
      message: "Der Nachname ist ein Pflichtfeld.",
    })
    .min(1, "Der Nachname ist ein Pflichtfeld.")
    .max(50, "Der Nachname darf maximal 50 Zeichen lang sein."),

  nickname: z
    .string()
    .max(50, "Der Spitzname darf maximal 50 Zeichen lang sein.")
    .nullable(),
});

export type UpdatePlayerDto = z.infer<typeof updatePlayerDtoSchema>;
