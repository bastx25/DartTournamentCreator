import z from "zod";

export const updateTournamentPlayerDtoSchema = z.object({
  tournamentId: z.number().int().min(1),
  playerId: z.number().int().min(1),
});

export type UpdateTournamentPlayerDto = z.infer<
  typeof updateTournamentPlayerDtoSchema
>;
