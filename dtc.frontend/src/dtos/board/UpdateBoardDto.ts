import z from "zod";

export const updateBoardDtoSchema = z.object({
  locationId: z.number().min(1, "Bitte einen Standort auswählen."),
  label: z.string().max(100).nullable().optional(),
  isActive: z.boolean(),
});

export type UpdateBoardDto = z.infer<typeof updateBoardDtoSchema>;
