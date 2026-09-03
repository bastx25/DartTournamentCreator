import { useEffect, useState } from "react";
import type { MatchDto } from "../../dtos/Match/MatchDto";
import { MatchStatus, matchStatusLabel } from "../../enums/MatchStatus";
import { getBoards } from "../../services/boardService";
import { formatTime } from "../../utils/formatTime";
import type { MatchSchedule } from "./DashboardPage";
import type { BoardDto } from "../../dtos/board/BoardDto";

interface DashboardMatchProps {
  match: MatchDto;
  index: number;
  locationId: number;
  scheduledTime: string;
  firstScore: number;
  secondScore: number;
  playerName: (match: MatchSchedule, index: number) => string;
}

export function DashboardMatch({
  match,
  index,
  locationId,
  scheduledTime,
  firstScore,
  secondScore,
  playerName,
}: DashboardMatchProps) {
  const firstParticipant = match.participants[0];
  const secondParticipant = match.participants[1];

  const [boards, setBoards] = useState<BoardDto[]>([]);

  useEffect(() => {
    const loadBoards = async () => {
      const boardsAtLocation = await getBoards(locationId);
      setBoards(boardsAtLocation);
    };

    loadBoards();
  }, [locationId]);

  const boardLabel = (boardId: number | null) => {
    const board = boards.find((board) => board.id === boardId);

    return board?.label ?? "";
  };

  return (
    <div className="grid gap-4 px-6 py-4 sm:grid-cols-[110px_minmax(0,1fr)_90px_110px] sm:items-center sm:px-8">
      <div>
        <p className="text-sm font-semibold text-gray-900">Match {index + 1}</p>
        <p className="mt-1 text-xs text-gray-500">
          {formatTime(scheduledTime)} Uhr
        </p>
      </div>

      <div className="min-w-0">
        <div className="grid gap-3 sm:grid-cols-[minmax(0,1fr)_auto_minmax(0,1fr)] sm:items-center">
          <div className="min-w-0">
            <p
              className={`truncate text-sm font-medium ${
                firstParticipant?.isWinner ? "text-green-600" : "text-gray-900"
              }`}
            >
              {playerName(match, 0)}
            </p>
          </div>

          <span className="whitespace-nowrap text-sm font-bold text-gray-900">
            {firstScore ?? 0} : {secondScore ?? 0}
          </span>

          <div className="min-w-0 sm:text-right">
            <p
              className={`truncate text-sm font-medium ${
                secondParticipant?.isWinner ? "text-green-600" : "text-gray-900"
              }`}
            >
              {playerName(match, 1)}
            </p>
          </div>
        </div>
      </div>

      <div className="text-sm font-medium text-gray-600 sm:text-center">
        Board {boardLabel(match.boardId)}
      </div>

      <div className="sm:text-right">
        <span
          className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${
            match.status === MatchStatus.InProgress
              ? "bg-amber-50 text-amber-700"
              : match.status === MatchStatus.Completed
                ? "bg-green-50 text-green-700"
                : match.status === MatchStatus.Cancelled
                  ? "bg-red-50 text-red-700"
                  : "bg-blue-50 text-blue-700"
          }`}
        >
          {matchStatusLabel(match.status)}
        </span>
      </div>
    </div>
  );
}
