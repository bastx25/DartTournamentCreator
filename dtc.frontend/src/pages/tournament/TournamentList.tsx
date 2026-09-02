import type { TournamentDto } from "../../dtos/Tournament/TournamentDto";
import { TournamentRow } from "./TournamentRow";

interface TournamentListProps {
  tournaments: TournamentDto[];
  onManageTournament: (tournament: TournamentDto) => void;
}

export function TournamentList({
  tournaments,
  onManageTournament,
}: TournamentListProps) {
  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-gray-200 px-4 py-4 sm:px-6">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-gray-900">
            Turniere
          </h1>

          <p className="mt-1 text-sm text-gray-500">
            {tournaments.length}{" "}
            {tournaments.length === 1 ? "Turnier" : "Turniere"} verfügbar
          </p>
        </div>
      </div>

      <div className="hidden grid-cols-[minmax(220px,1.5fr)_170px_180px_130px_110px_48px] items-center border-b border-gray-200 bg-gray-50 px-6 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500 lg:grid">
        <span>Turnier</span>
        <span>Start</span>
        <span>Modus</span>
        <span>Status</span>
        <span>Runden / Matches</span>
        <span className="text-right">Verwaltung</span>
      </div>

      <div className="divide-y divide-gray-100">
        {tournaments.map((tournament) => (
          <TournamentRow
            key={tournament.id}
            tournament={tournament}
            onManage={onManageTournament}
          />
        ))}
      </div>

      {tournaments.length === 0 && (
        <div className="px-6 py-12 text-center">
          <p className="text-sm font-medium text-gray-700">
            Noch keine Turniere vorhanden.
          </p>
          <p className="mt-1 text-sm text-gray-500">
            Erstelle ein Turnier, um es hier anzuzeigen.
          </p>
        </div>
      )}
    </section>
  );
}
