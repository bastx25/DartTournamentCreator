import axios from "axios";
import type { MatchDto } from "../dtos/match/MatchDto";

export async function getMatchesByRoundId(
  roundId: number,
): Promise<MatchDto[]> {
  const response = await axios.get<MatchDto[]>(
    `/api/rounds/${roundId}/matches`,
  );
  return response.data ?? [];
}
