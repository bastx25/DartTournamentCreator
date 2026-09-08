import { Settings } from "lucide-react";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import { TournamentMode } from "../../enums/TournamentMode";
import {
  tournamentStatusClasses,
  tournamentStatusLabels,
} from "../../enums/TournamentStatus";

interface TournamentRowProps {
  tournament: TournamentDto;
  onManage: (tournament: TournamentDto) => void;
}

const modeLabels: Record<TournamentMode, string> = {
  [TournamentMode.GroupStage]: "Gruppenphase",
  [TournamentMode.GrouStageandKnockout]: "Gruppenphase & K.-o.",
};

function formatStartDate(startDate: string) {
  return new Intl.DateTimeFormat("de-DE", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(startDate));
}

export function TournamentRow({ tournament, onManage }: TournamentRowProps) {
  const matchCount = tournament.rounds?.reduce(
    (count, round) => count + round.matches.length,
    0,
  );

  return (
    <div className="grid gap-4 px-4 py-4 transition-colors hover:bg-gray-50 sm:px-6 lg:grid-cols-[minmax(220px,1.5fr)_170px_180px_130px_110px_80px] lg:items-center lg:gap-0">
      <div className="min-w-0">
        <div className="flex items-center gap-2">
          <span className="truncate text-sm font-semibold text-gray-900">
            {tournament.name}
          </span>

          <span
            className={`shrink-0 rounded-full px-2 py-0.5 text-[11px] font-medium lg:hidden ${tournamentStatusClasses[tournament.status]}`}
          >
            {tournamentStatusLabels[tournament.status]}
          </span>
        </div>

        {tournament.description && (
          <p className="mt-1 truncate text-xs text-gray-500">
            {tournament.description}
          </p>
        )}

        <div className="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-500 lg:hidden">
          <span>{formatStartDate(tournament.startDate)}</span>
          <span>{modeLabels[tournament.mode]}</span>
          <span>
            {tournament.rounds?.length}{" "}
            {tournament.rounds?.length === 1 ? "Runde" : "Runden"}
          </span>
          <span>
            {matchCount} {matchCount === 1 ? "Match" : "Matches"}
          </span>
        </div>
      </div>

      <div className="hidden text-sm text-gray-600 lg:block">
        {formatStartDate(tournament.startDate)}
      </div>

      <div className="hidden text-sm text-gray-600 lg:block">
        {modeLabels[tournament.mode]}
      </div>

      <div className="hidden lg:block">
        <span
          className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${tournamentStatusClasses[tournament.status]}`}
        >
          {tournamentStatusLabels[tournament.status]}
        </span>
      </div>

      <div className="hidden text-sm text-gray-600 lg:block">
        {tournament.rounds?.length} / {matchCount}
      </div>

      <div className="flex justify-end">
        <button
          type="button"
          onClick={() => onManage(tournament)}
          className="inline-flex h-9 w-9 items-center justify-center rounded-lg text-gray-400 transition-colors hover:bg-blue-50 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
          title={`Turnier "${tournament.name}" verwalten`}
          aria-label={`Turnier "${tournament.name}" verwalten`}
        >
          <Settings
            className="h-5 w-5 text-gray-500"
            strokeWidth={1.8}
            aria-hidden="true"
          />
        </button>
      </div>
    </div>
  );
}
