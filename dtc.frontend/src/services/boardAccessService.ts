import axios from "axios";
import type { BoardDto } from "../dtos/board/BoardDto";
import type { MatchStatus } from "../enums/MatchStatus";
import type { MatchParticipantDto } from "../dtos/MatchParticipant/MatchParticipantDto";

export interface BoardMatchDto {
  matchId: number;
  roundId: number;
  roundName: string | null;
  tournamentName: string;
  plannedStart: string | null;
  actualStart: string | null;
  status: MatchStatus;
  participants: MatchParticipantDto[];
}

export interface FinishBoardMatchDto {
  participants: Array<{
    participantId: number;
    score: number;
  }>;
}

export async function getBoardForAccess(boardId: number): Promise<BoardDto> {
  const response = await axios.get<BoardDto>(`/api/board-access/boards/${boardId}`);
  return response.data;
}

export async function getBoardMatches(boardId: number): Promise<BoardMatchDto[]> {
  const response = await axios.get<BoardMatchDto[]>(
    `/api/board-access/boards/${boardId}/matches`,
  );
  return response.data;
}

export async function startBoardMatch(
  boardId: number,
  matchId: number,
): Promise<BoardMatchDto> {
  const response = await axios.post<BoardMatchDto>(
    `/api/board-access/boards/${boardId}/matches/${matchId}/start`,
  );
  return response.data;
}

export async function finishBoardMatch(
  boardId: number,
  matchId: number,
  data: FinishBoardMatchDto,
): Promise<BoardMatchDto> {
  const response = await axios.post<BoardMatchDto>(
    `/api/board-access/boards/${boardId}/matches/${matchId}/finish`,
    data,
    { headers: { "Content-Type": "application/json" } },
  );
  return response.data;
}
