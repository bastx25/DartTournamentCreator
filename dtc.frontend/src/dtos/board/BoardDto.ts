import z from "zod";

export const boardDtoSchema = z.object({
  id: z.number(),
  locationId: z.number(),
  label: z.string().nullable().optional(),
  isActive: z.boolean(),
});

export type BoardDto = z.infer<typeof boardDtoSchema>;
