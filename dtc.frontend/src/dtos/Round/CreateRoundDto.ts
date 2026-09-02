import z from "zod";

export const createRoundDtoSchema = z.object({
  tournamentId: z.number({
    message: "TournamentId ist ein Pflichtfeld.",
  }),

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

  plannedStart: z.string().datetime({ offset: true }),

  plannedEnd: z.string().datetime({ offset: true }).nullable(),

  status: z
    .enum(["Scheduled", "InProgress", "Completed", "Cancelled"])
    .default("Scheduled"),
});

export type CreateRoundDto = z.infer<typeof createRoundDtoSchema>;
