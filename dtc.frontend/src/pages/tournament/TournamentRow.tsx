import type { TournamentDto } from "../../dtos/Tournament/TournamentDto";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";

interface TournamentRowProps {
  tournament: TournamentDto;
  onManage: (tournament: TournamentDto) => void;
}

const statusLabels: Record<TournamentStatus, string> = {
  [TournamentStatus.Draft]: "Entwurf",
  [TournamentStatus.Scheduled]: "Geplant",
  [TournamentStatus.InProgress]: "Laufend",
  [TournamentStatus.Completed]: "Abgeschlossen",
  [TournamentStatus.Cancelled]: "Abgebrochen",
};

const modeLabels: Record<TournamentMode, string> = {
  [TournamentMode.GroupStage]: "Gruppenphase",
  [TournamentMode.GrouStageandKnockout]: "Gruppenphase & K.-o.",
};

const statusClasses: Record<TournamentStatus, string> = {
  [TournamentStatus.Draft]: "bg-gray-100 text-gray-700",
  [TournamentStatus.Scheduled]: "bg-blue-50 text-blue-700",
  [TournamentStatus.InProgress]: "bg-green-50 text-green-700",
  [TournamentStatus.Completed]: "bg-purple-50 text-purple-700",
  [TournamentStatus.Cancelled]: "bg-red-50 text-red-700",
};

function formatStartDate(startDate: string) {
  return new Intl.DateTimeFormat("de-DE", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(startDate));
}

export function TournamentRow({ tournament, onManage }: TournamentRowProps) {
  const matchCount = tournament.rounds.reduce(
    (count, round) => count + round.matches.length,
    0,
  );

  return (
    <div className="grid gap-4 px-4 py-4 transition-colors hover:bg-gray-50 sm:px-6 lg:grid-cols-[minmax(220px,1.5fr)_170px_180px_130px_110px_48px] lg:items-center lg:gap-0">
      <div className="min-w-0">
        <div className="flex items-center gap-2">
          <span className="truncate text-sm font-semibold text-gray-900">
            {tournament.name}
          </span>

          <span
            className={`shrink-0 rounded-full px-2 py-0.5 text-[11px] font-medium lg:hidden ${statusClasses[tournament.status]}`}
          >
            {statusLabels[tournament.status]}
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
            {tournament.rounds.length}{" "}
            {tournament.rounds.length === 1 ? "Runde" : "Runden"}
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
          className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${statusClasses[tournament.status]}`}
        >
          {statusLabels[tournament.status]}
        </span>
      </div>

      <div className="hidden text-sm text-gray-600 lg:block">
        {tournament.rounds.length} / {matchCount}
      </div>

      <div className="flex justify-end">
        <button
          type="button"
          onClick={() => onManage(tournament)}
          className="inline-flex h-9 w-9 items-center justify-center rounded-lg text-gray-400 transition-colors hover:bg-blue-50 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
          title={`Turnier "${tournament.name}" verwalten`}
          aria-label={`Turnier "${tournament.name}" verwalten`}
        >
          <svg
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            className="h-5 w-5"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M10.3 3.2h3.4l.5 2.1c.5.2 1 .4 1.4.8l2.1-.8 1.7 3-1.6 1.4c.1.5.1 1 0 1.5l1.6 1.4-1.7 3-2.1-.8c-.4.3-.9.6-1.4.8l-.5 2.1h-3.4l-.5-2.1c-.5-.2-1-.4-1.4-.8l-2.1.8-1.7-3 1.6-1.4a6 6 0 0 1 0-1.5L4.6 8.3l1.7-3 2.1.8c.4-.3.9-.6 1.4-.8l.5-2.1Z"
            />
            <circle cx="12" cy="12" r="2.5" />
          </svg>
        </button>
      </div>
    </div>
  );
}
