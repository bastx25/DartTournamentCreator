import z from "zod";

export const createLocationDtoSchema = z.object({
  name: z
    .string({
      message: "Name ist ein Pflichtfeld.",
    })
    .min(1, "Name ist ein Pflichtfeld.")
    .max(100, "Der Name darf maximal 100 Zeichen lang sein."),

  address: z
    .string()
    .max(200, "Die Adresse darf maximal 200 Zeichen lang sein.")
    .nullable(),
});

export type CreateLocationDto = z.infer<typeof createLocationDtoSchema>;
