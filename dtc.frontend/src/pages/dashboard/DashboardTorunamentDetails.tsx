import type { TournamentDto } from "../../dtos/Tournament/TournamentDto";
import { tournamentStatusLabel } from "../../enums/TournamentStatus";
import { formatDate } from "../../utils/formatDate";

interface DashboardTournamentDetailsProps {
  tournament: TournamentDto;
  roundsLength: number;
  totalMatches: number;
  activeRoundCount: number;
}

export function DashboardTournamentDetails({
  tournament,
  roundsLength,
  totalMatches,
  activeRoundCount,
}: DashboardTournamentDetailsProps) {
  return (
    <section className="mb-6 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-xl font-semibold tracking-tight text-gray-900">
                {tournament.name}
              </h2>
              <span className="rounded-full bg-blue-50 px-2.5 py-1 text-xs font-medium text-blue-700">
                {tournamentStatusLabel(tournament.status)}
              </span>
            </div>
            <p className="mt-2 text-sm text-gray-500">
              Start: {formatDate(tournament.startDate)}
              {tournament.description ? ` · ${tournament.description}` : ""}
            </p>
          </div>

          <dl className="grid grid-cols-3 gap-4 text-sm sm:min-w-96">
            <div>
              <dt className="text-gray-500">Runden</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {roundsLength}
              </dd>
            </div>
            <div>
              <dt className="text-gray-500">Matches</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {totalMatches}
              </dd>
            </div>
            <div>
              <dt className="text-gray-500">Laufende Runden</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {activeRoundCount}
              </dd>
            </div>
          </dl>
        </div>
      </div>
    </section>
  );
}
