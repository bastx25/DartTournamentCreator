import z from "zod";

export const createBoardDtoSchema = z.object({
  locationId: z.number().min(1, "Bitte einen Standort auswählen."),
  label: z.string().max(100).nullable().optional(),
  isActive: z.boolean(),
});

export type CreateBoardDto = z.infer<typeof createBoardDtoSchema>;
