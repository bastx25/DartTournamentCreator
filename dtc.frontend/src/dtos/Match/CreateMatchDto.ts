import z from "zod";

export const createMatchDtoSchema = z.object({
  roundId: z.number({
    message: "RoundId ist ein Pflichtfeld.",
  }),

  boardId: z.number().nullable(),

  status: z
    .enum(["Scheduled", "InProgress", "Completed", "Cancelled"])
    .default("Scheduled"),
});

export type CreateMatchDto = z.infer<typeof createMatchDtoSchema>;
