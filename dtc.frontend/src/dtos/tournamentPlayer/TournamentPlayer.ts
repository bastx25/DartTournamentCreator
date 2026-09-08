import z from "zod";

export const tournamentPlayerDtoSchema = z.object({
  id: z.number().int(),
  tournamentId: z.number().int(),
  playerId: z.number().int(),
});

export type TournamentPlayerDto = z.infer<typeof tournamentPlayerDtoSchema>;
