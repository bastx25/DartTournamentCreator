import z from "zod";

export const locationDtoSchema = z.object({
  id: z.number(),
  name: z.string(),
  address: z.string().nullable(),
});

export type LocationDto = z.infer<typeof locationDtoSchema>;
