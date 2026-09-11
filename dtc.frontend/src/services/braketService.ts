import axios from "axios";
import type { MatchDto } from "../dtos/match/MatchDto";

export async function getMatchesByBraketId(
  braketId: number,
): Promise<MatchDto[]> {
  const response = await axios.get<MatchDto[]>(
    `/api/brakets/${braketId}/matches`,
  );
  return response.data ?? [];
}
