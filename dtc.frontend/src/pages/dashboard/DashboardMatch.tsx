import type { MatchDto } from "../../dtos/Match/MatchDto";
import { MatchStatus, matchStatusLabel } from "../../enums/MatchStatus";
import { formatTime } from "../../utils/formatTime";
import type { MatchSchedule } from "./DashboardPage";

interface DashboardMatchProps {
  match: MatchDto;
  index: number;
  scheduledTime: string;
  firstScore: number;
  secondScore: number;
  playerName: (match: MatchSchedule, index: number) => string;
}

export function DashboardMatch({
  match,
  index,
  scheduledTime,
  firstScore,
  secondScore,
  playerName,
}: DashboardMatchProps) {
  return (
    <div
      key={match.id}
      className="grid gap-4 px-6 py-4 sm:grid-cols-[110px_minmax(0,1fr)_120px] sm:items-center sm:px-8"
    >
      <div>
        <p className="text-sm font-semibold text-gray-900">Match {index + 1}</p>
        <p className="mt-1 text-xs text-gray-500">
          {formatTime(scheduledTime)} Uhr
        </p>
      </div>

      <div className="min-w-0">
        <div className="grid gap-2 sm:grid-cols-[minmax(0,1fr)_auto_minmax(0,1fr)] sm:items-center">
          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-gray-900">
              {playerName(match, 0)}
            </p>
            {firstScore !== null && (
              <p className="mt-1 text-xs text-gray-500">Score: {firstScore}</p>
            )}
          </div>

          <span className="text-xs font-semibold uppercase tracking-wide text-gray-400">
            vs.
          </span>

          <div className="min-w-0 sm:text-right">
            <p className="truncate text-sm font-medium text-gray-900">
              {playerName(match, 1)}
            </p>
            {secondScore !== null && (
              <p className="mt-1 text-xs text-gray-500">Score: {secondScore}</p>
            )}
          </div>
        </div>
      </div>

      <div>{match.boardId}</div>

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
