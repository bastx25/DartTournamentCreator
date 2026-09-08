import z from "zod";

export const createTournamentPlayerDtoSchema = z.object({
  tournamentId: z.number().int().min(1),
  playerId: z.number().int().min(1),
});

export type CreateTournamentPlayerDto = z.infer<
  typeof createTournamentPlayerDtoSchema
>;
