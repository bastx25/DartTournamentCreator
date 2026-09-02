import z from "zod";
import { RoundStatus } from "../../enums/RoundStatus";

export const updateRoundDtoSchema = z.object({
  locationId: z.number({
    message: "LocationId ist ein Pflichtfeld.",
  }),

  sequence: z
    .number()
    .min(1, "Die Sequenz muss mindestens 1 sein.")
    .max(100, "Die Sequenz darf maximal 100 sein."),

  name: z
    .string()
    .max(50, "Der Rundenname darf maximal 50 Zeichen lang sein.")
    .nullable(),

  plannedStart: z.iso.datetime({ offset: true }),

  plannedEnd: z.iso.datetime({ offset: true }).nullable(),

  status: z.enum(RoundStatus).default(RoundStatus.Scheduled),
});

export type UpdateRoundDto = z.infer<typeof updateRoundDtoSchema>;
