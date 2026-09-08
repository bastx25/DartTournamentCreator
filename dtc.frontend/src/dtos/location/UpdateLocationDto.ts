import z from "zod";

export const updateLocationDtoSchema = z.object({
  name: z.string().min(1).max(100),
  address: z.string().max(250).nullable(),
});

export type UpdateLocationDto = z.infer<typeof updateLocationDtoSchema>;
