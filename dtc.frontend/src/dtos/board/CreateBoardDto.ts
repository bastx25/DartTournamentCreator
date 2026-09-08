import z from "zod";

export const createBoardDtoSchema = z.object({
  locationId: z.number(),
  number: z.number().int().min(1),
  label: z.string().max(100).nullable(),
  isActive: z.boolean(),
});

export type CreateBoardDto = z.infer<typeof createBoardDtoSchema>;
