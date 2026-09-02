import z from "zod";
import { boardDtoSchema } from "../board/BoardDto";

export const locationDtoSchema = z.object({
  id: z.number(),
  name: z.string(),
  address: z.string().nullable(),
  boards: z.array(boardDtoSchema),
});

export type LocationDto = z.infer<typeof locationDtoSchema>;
