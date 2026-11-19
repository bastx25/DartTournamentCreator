import type { Dispatch, SetStateAction } from "react";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import type { GroupDto } from "../../dtos/group/GroupDto";
import type { BraketDto } from "../../dtos/brakets/BraketDto";

interface DashboardHeaderProps {
  effectiveTournamentId: number | null;
  setSelectedTournamentId: Dispatch<SetStateAction<number | null>>;
  tournamentsLoading: boolean;
  tournaments: TournamentDto[];
  selectedFilter: string;
  onFilterChange: (filter: string) => void;
  groups: GroupDto[];
  brakets: BraketDto[];
}

export function DashboardHeader({
  effectiveTournamentId,
  setSelectedTournamentId,
  tournamentsLoading,
  tournaments,
  selectedFilter,
  onFilterChange,
  groups,
  brakets,
}: DashboardHeaderProps) {
  return (
    <div
      key="Header"
      className="mb-8 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between"
    >
      <div>
        <h1 className="text-3xl font-bold tracking-tight text-gray-900">
          Dashboard
        </h1>
        <p className="mt-2 text-sm text-gray-600">
          Runden und Matches des aktuell ausgewählten aktiven Turniers.
        </p>
      </div>

      <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
        <div className="w-full sm:w-64">
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

        <div className="w-full sm:w-64">
          <label
            htmlFor="dashboard-filter"
            className="block text-sm font-medium text-gray-900"
          >
            Filter nach Phase / Gruppe
          </label>
          <select
            id="dashboard-filter"
            value={selectedFilter}
            onChange={(event) => onFilterChange(event.target.value)}
            disabled={
              tournamentsLoading ||
              (groups.length === 0 && brakets.length === 0)
            }
            className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
          >
            <option value="all">🎯 Alle Matches</option>

            {groups.length > 0 && (
              <optgroup label="Gruppenphase">
                <option value="group-all">Alle Gruppen</option>
                {groups.map((group) => (
                  <option key={`group-${group.id}`} value={`group-${group.id}`}>
                    {group.name}
                  </option>
                ))}
              </optgroup>
            )}

            {brakets.length > 0 && (
              <optgroup label="K.-o.-Phase">
                <option value="knockout-all">K.-o.-Phase (Alle Runden)</option>
                {brakets.map((braket) => (
                  <option
                    key={`braket-${braket.id}`}
                    value={`braket-${braket.id}`}
                  >
                    {braket.name ?? `Runde ${braket.sequence}`}
                  </option>
                ))}
              </optgroup>
            )}
          </select>
        </div>
      </div>
    </div>
  );
}
