import type { Dispatch, SetStateAction } from "react";
import type { TournamentDto } from "../../dtos/Tournament/TournamentDto";

interface DashboardHeaderProps {
  effectiveTournamentId: number;
  setSelectedTournamentId: Dispatch<SetStateAction<number | null>>;
  tournamentsLoading: boolean;
  tournaments: TournamentDto[];
}

export function DashboardHeader({
  effectiveTournamentId,
  setSelectedTournamentId,
  tournamentsLoading,
  tournaments,
}: DashboardHeaderProps) {
  return (
    <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 className="text-3xl font-bold tracking-tight text-gray-900">
          Dashboard
        </h1>
        <p className="mt-2 text-sm text-gray-600">
          Runden und Matches des aktuell ausgewählten aktiven Turniers.
        </p>
      </div>

      <div className="w-full sm:w-96">
        <label
          htmlFor="dashboard-tournament"
          className="block text-sm font-medium text-gray-900"
        >
          Aktives Turnier
        </label>
        <select
          id="dashboard-tournament"
          value={effectiveTournamentId ?? ""}
          onChange={(event) =>
            setSelectedTournamentId(
              event.target.value ? Number(event.target.value) : null,
            )
          }
          disabled={tournamentsLoading || tournaments.length === 0}
          className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
        >
          {tournaments.length === 0 ? (
            <option value="">Keine aktiven Turniere</option>
          ) : (
            tournaments.map((item) => (
              <option key={item.id} value={item.id}>
                {item.name}
              </option>
            ))
          )}
        </select>
      </div>
    </div>
  );
}
