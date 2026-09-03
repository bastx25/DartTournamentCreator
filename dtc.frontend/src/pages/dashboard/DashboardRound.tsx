import type { BoardDto } from "../../dtos/board/BoardDto";
import type { MatchDto } from "../../dtos/Match/MatchDto";
import type { RoundDto } from "../../dtos/Round/RoundDto";
import { roundStatusLabel } from "../../enums/RoundStatus";
import { formatDate } from "../../utils/formatDate";
import { DashboardMatch } from "./DashboardMatch";
import type { MatchSchedule } from "./DashboardPage";

interface DashboardRoundProps {
  round: RoundDto;
  sortedMatches: MatchDto[];
  boards: BoardDto[];
  playerName: (match: MatchSchedule, index: number) => string;
  matchScore: (match: MatchSchedule, index: number) => number;
  matchTime: (match: MatchSchedule, roundStart: string) => string;
}

export function DashboardRound({
  round,
  sortedMatches,
  boards,
  playerName,
  matchScore,
  matchTime,
}: DashboardRoundProps) {
  return (
    <section
      key={round.id}
      className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm"
    >
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-lg font-semibold text-gray-900">
                {round.name ?? `Runde ${round.sequence}`}
              </h2>
              <span className="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-600">
                {roundStatusLabel(round.status)}
              </span>
            </div>
            <p className="mt-1 text-sm text-gray-500">
              Geplanter Start: {formatDate(round.plannedStart)}
            </p>
          </div>
          <span className="text-sm font-medium text-gray-500">
            {round.matches.length}{" "}
            {round.matches.length === 1 ? "Match" : "Matches"}
          </span>
        </div>
      </div>

      <div className="divide-y divide-gray-100">
        {sortedMatches.map((match, index) => {
          const scheduledTime = matchTime(match, round.plannedStart);
          const firstScore = matchScore(match, 0);
          const secondScore = matchScore(match, 1);

          return (
            <DashboardMatch
              match={match}
              index={index}
              boards={boards}
              scheduledTime={scheduledTime}
              firstScore={firstScore}
              secondScore={secondScore}
              playerName={playerName}
            />
          );
        })}
      </div>
    </section>
  );
}
