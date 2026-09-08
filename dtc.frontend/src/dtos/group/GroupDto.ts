import z from "zod";
import { groupPlayerDtoSchema } from "../groupPlayer/GroupPlayerDto";
import { matchDtoSchema } from "../match/MatchDto";

export const groupDtoSchema = z.object({
  id: z.number(),
  tournamentId: z.number(),
  sequence: z.number(),
  name: z.string(),
  qualifiersCount: z.number(),
  groupPlayers: z.array(groupPlayerDtoSchema).nullable(),
  matches: z.array(matchDtoSchema).nullable(),
});

export type GroupDto = z.infer<typeof groupDtoSchema>;
