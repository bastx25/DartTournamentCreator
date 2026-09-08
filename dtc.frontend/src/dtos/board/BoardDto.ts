import z from "zod";
import { locationDtoSchema } from "../location/LocationDto";

export const boardDtoSchema = z.object({
  id: z.number(),
  location: locationDtoSchema.nullable(),
  number: z.number(),
  label: z.string().nullable(),
  isActive: z.boolean(),
});

export type BoardDto = z.infer<typeof boardDtoSchema>;
