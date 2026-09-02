import z from "zod";

export const updateMatchDtoSchema = z.object({
  boardId: z.number().nullable(),

  status: z.enum(["Scheduled", "InProgress", "Completed", "Cancelled"]),

  actualStart: z.string().datetime({ offset: true }).nullable(),

  actualEnd: z.string().datetime({ offset: true }).nullable(),
});

export type UpdateMatchDto = z.infer<typeof updateMatchDtoSchema>;
