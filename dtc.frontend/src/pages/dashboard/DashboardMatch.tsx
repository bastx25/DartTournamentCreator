import { useEffect, useState } from "react";
import { MatchStatus, matchStatusLabel } from "../../enums/MatchStatus";
import { getBoard } from "../../services/boardService";
import { formatTime } from "../../utils/formatTime";
import type { MatchSchedule } from "./DashboardPage";
import type { BoardDto } from "../../dtos/board/BoardDto";
import type { MatchDto } from "../../dtos/match/MatchDto";

interface DashboardMatchProps {
  match: MatchDto;
  index: number;
  playerName: (match: MatchSchedule, index: number) => string;
}

export function DashboardMatch({
  match,
  index,
  playerName,
}: DashboardMatchProps) {
  const firstParticipant = match.participants[0];
  const secondParticipant = match.participants[1];

  const [board, setBoard] = useState<BoardDto | null>(null);

  useEffect(() => {
    const loadBoard = async () => {
      if (match.boardId != null) {
        const boardFromMatch = await getBoard(match.boardId);
        setBoard(boardFromMatch);
      }
    };

    loadBoard();
  }, [match.boardId]);

  const boardLabel = board?.label;

  return (
    <div className="grid gap-4 px-6 py-4 sm:grid-cols-[110px_minmax(0,1fr)_90px_110px] sm:items-center sm:px-8">
      <div>
        <p className="text-sm font-semibold text-gray-900">Match {index + 1}</p>
        <p className="mt-1 text-xs text-gray-500">
          {formatTime(match.actualStart)} Uhr
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
            {match.participants[0].score ?? 0} :{" "}
            {match.participants[1].score ?? 0}
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
        Board {boardLabel}
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
