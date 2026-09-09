import axios from "axios";
import type { MatchDto } from "../dtos/match/MatchDto";

export async function getMatchesByGroupId(
  groupId: number,
): Promise<MatchDto[]> {
  const response = await axios.get<MatchDto[]>(
    `/api/groups/${groupId}/matches`,
  );
  return response.data ?? [];
}
