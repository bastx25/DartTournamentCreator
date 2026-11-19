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
  tournamentCodeQuery: string;
  onTournamentCodeQueryChange: (query: string) => void;
  onSearchCodeSubmit: (e: React.FormEvent) => void;
  activeCode: string | null;
  onClearCode: () => void;
  isCodeSearching?: boolean;
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
  tournamentCodeQuery,
  onTournamentCodeQueryChange,
  onSearchCodeSubmit,
  activeCode,
  onClearCode,
  isCodeSearching = false,
}: DashboardHeaderProps) {
  return (
    <div key="Header" className="mb-8">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-gray-900">
            Dashboard
          </h1>
          <p className="mt-2 text-sm text-gray-600">
            {activeCode
              ? `Turnier mit Code ${activeCode}`
              : "Runden und Matches des aktuell ausgewählten aktiven Turniers."}
          </p>
        </div>

        <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
          {/* Tournament Code Search Form */}
          <form onSubmit={onSearchCodeSubmit} className="w-full sm:w-64">
            <label
              htmlFor="dashboard-code-input"
              className="block text-sm font-medium text-gray-900"
            >
              Turnier-Code
            </label>
            <div className="mt-2 flex gap-1.5">
              <input
                id="dashboard-code-input"
                type="text"
                placeholder="DART-XXXXXX"
                value={tournamentCodeQuery}
                onChange={(e) =>
                  onTournamentCodeQueryChange(e.target.value.toUpperCase())
                }
                className="block w-full font-mono uppercase rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 placeholder-gray-400 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
              />
              <button
                type="submit"
                disabled={!tournamentCodeQuery.trim() || isCodeSearching}
                className="rounded-lg bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {isCodeSearching ? "..." : "Öffnen"}
              </button>
            </div>
          </form>

          {/* Active Tournament Selector */}
          <div className="w-full sm:w-64">
            <label
              htmlFor="dashboard-tournament"
              className="block text-sm font-medium text-gray-900"
            >
              Aktives Turnier
            </label>
            <select
              id="dashboard-tournament"
              value={activeCode ? "" : (effectiveTournamentId ?? "")}
              onChange={(event) => {
                onClearCode();
                setSelectedTournamentId(
                  event.target.value ? Number(event.target.value) : null,
                );
              }}
              disabled={tournamentsLoading || tournaments.length === 0}
              className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
            >
              {activeCode && (
                <option value="" disabled>
                  (Turnier per Code geladen)
                </option>
              )}
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

          {/* Phase / Group Filter */}
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
                  <option value="knockout-all">Alle K.-o.-Runden</option>
                  {brakets.map((braket) => (
                    <option
                      key={`braket-${braket.id}`}
                      value={`braket-${braket.id}`}
                    >
                      {braket.name}
                    </option>
                  ))}
                </optgroup>
              )}
            </select>
          </div>
        </div>
      </div>

      {/* Code Active Banner */}
      {activeCode && (
        <div className="mt-4 flex items-center justify-between rounded-lg border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-800">
          <div className="flex items-center gap-2">
            <span>🎯</span>
            <span>
              Turnier mit Code <strong className="font-mono">{activeCode}</strong> wird angezeigt.
            </span>
          </div>
          <button
            type="button"
            onClick={onClearCode}
            className="rounded font-semibold text-blue-700 hover:text-blue-900 underline transition"
          >
            Zurück zu aktiven Turnieren
          </button>
        </div>
      )}
    </div>
  );
}
