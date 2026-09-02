import z from "zod";

export const createBoardDtoSchema = z.object({
  locationId: z.number({
    message: "LocationId ist ein Pflichtfeld.",
  }),

  number: z
    .number()
    .min(1, "Die Boardnummer muss mindestens 1 sein.")
    .max(100, "Die Boardnummer darf maximal 100 sein."),

  label: z
    .string()
    .max(50, "Das Label darf maximal 50 Zeichen lang sein.")
    .nullable(),

  isActive: z.boolean().default(true),
});

export type CreateBoardDto = z.infer<typeof createBoardDtoSchema>;
