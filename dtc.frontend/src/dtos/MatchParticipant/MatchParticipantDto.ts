import z from "zod";

export const boardDtoSchema = z.object({
  id: z.number(),
  locationId: z.number(),
  number: z.number(),
  label: z.string().nullable(),
  isActive: z.boolean(),
});

export type BoardDto = z.infer<typeof boardDtoSchema>;
