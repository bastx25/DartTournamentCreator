import { useState } from "react";
import type { PublicTournamentDto } from "../../dtos/public/PublicTournamentDto";
import type { GroupDto } from "../../dtos/group/GroupDto";
import type { BraketDto } from "../../dtos/brakets/BraketDto";

export interface SavedTournamentItem {
  code: string;
  tournament: PublicTournamentDto | null;
  error?: string | null;
  loading?: boolean;
}

interface DashboardHeaderProps {
  savedTournaments: SavedTournamentItem[];
  activeCode: string | null;
  onSelectTournament: (code: string) => void;
  onRemoveTournament: (code: string) => void;
  onAddTournamentCode: (code: string) => Promise<boolean>;
  isAddingCode: boolean;
  addCodeError: string | null;
  setAddCodeError: (error: string | null) => void;
  selectedFilter: string;
  onFilterChange: (filter: string) => void;
  groups: GroupDto[];
  brakets: BraketDto[];
}

export function DashboardHeader({
  savedTournaments,
  activeCode,
  onSelectTournament,
  onRemoveTournament,
  onAddTournamentCode,
  isAddingCode,
  addCodeError,
  setAddCodeError,
  selectedFilter,
  onFilterChange,
  groups,
  brakets,
}: DashboardHeaderProps) {
  const [inputCode, setInputCode] = useState("");

  const handleAddSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = inputCode.trim().toUpperCase();
    if (!trimmed) return;

    const success = await onAddTournamentCode(trimmed);
    if (success) {
      setInputCode("");
    }
  };

  return (
    <div className="mb-8 space-y-6">
      {/* Top row: Title & Actions */}
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <h1 className="text-3xl font-extrabold tracking-tight text-gray-900">
            Dashboard
          </h1>
          <p className="mt-1 text-sm text-gray-600">
            Öffentliche Turnieransicht, Spielpläne und Board-Ergebnisse.
          </p>
        </div>

        {/* Action Controls */}
        <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
          {/* Tournament Code Add Form */}
          <form onSubmit={handleAddSubmit} className="w-full sm:w-80">
            <label
              htmlFor="dashboard-add-code"
              className="block text-xs font-semibold uppercase tracking-wider text-gray-700"
            >
              Turnier hinzufügen
            </label>
            <div className="mt-1.5 flex gap-1.5">
              <input
                id="dashboard-add-code"
                type="text"
                placeholder="DART-XXXXXX"
                value={inputCode}
                onChange={(e) => {
                  setInputCode(e.target.value.toUpperCase());
                  if (addCodeError) setAddCodeError(null);
                }}
                className="block w-full font-mono uppercase rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 placeholder-gray-400 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
              />
              <button
                type="submit"
                disabled={!inputCode.trim() || isAddingCode}
                className="inline-flex shrink-0 items-center justify-center rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 active:scale-95 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {isAddingCode ? "Lädt..." : "+ Hinzufügen"}
              </button>
            </div>
          </form>

          {/* Phase / Group Filter (Only relevant if active tournament exists) */}
          {activeCode && (groups.length > 0 || brakets.length > 0) && (
            <div className="w-full sm:w-64">
              <label
                htmlFor="dashboard-filter"
                className="block text-xs font-semibold uppercase tracking-wider text-gray-700"
              >
                Filter nach Phase / Gruppe
              </label>
              <select
                id="dashboard-filter"
                value={selectedFilter}
                onChange={(event) => onFilterChange(event.target.value)}
                className="mt-1.5 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
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
          )}
        </div>
      </div>

      {/* Add Code Error Message */}
      {addCodeError && (
        <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span>⚠️</span>
            <span>{addCodeError}</span>
          </div>
          <button
            type="button"
            onClick={() => setAddCodeError(null)}
            className="text-xs font-bold text-red-600 hover:text-red-800"
          >
            ✕ Schließen
          </button>
        </div>
      )}

      {/* Saved Tournaments Chips / Bar */}
      {savedTournaments.length > 0 && (
        <div className="rounded-xl border border-gray-200 bg-white p-3 shadow-sm">
          <div className="flex items-center justify-between gap-2 pb-2 border-b border-gray-100">
            <span className="text-xs font-bold uppercase tracking-wider text-gray-500">
              Gespeicherte Turniere ({savedTournaments.length})
            </span>
            <span className="text-xs text-gray-400">
              Temporär in Browser-Cookies gespeichert
            </span>
          </div>

          <div className="mt-3 flex flex-wrap gap-2">
            {savedTournaments.map((item) => {
              const isActive = item.code === activeCode;
              const hasError = Boolean(item.error);

              return (
                <div
                  key={item.code}
                  className={`inline-flex items-center gap-2 rounded-lg border px-3 py-1.5 text-sm transition ${
                    isActive
                      ? "border-blue-600 bg-blue-50 text-blue-900 shadow-sm"
                      : hasError
                        ? "border-red-200 bg-red-50/50 text-red-800 hover:bg-red-50"
                        : "border-gray-200 bg-gray-50 text-gray-700 hover:bg-gray-100"
                  }`}
                >
                  <button
                    type="button"
                    onClick={() => onSelectTournament(item.code)}
                    className="flex items-center gap-2 text-left font-medium outline-none"
                  >
                    <span>{hasError ? "⚠️" : "🎯"}</span>
                    <span>
                      {item.tournament?.name || item.code}
                    </span>
                    <span className="font-mono text-xs opacity-75">
                      [{item.code}]
                    </span>
                  </button>

                  <button
                    type="button"
                    onClick={(e) => {
                      e.stopPropagation();
                      onRemoveTournament(item.code);
                    }}
                    title={`Turnier ${item.code} aus Dashboard entfernen`}
                    className="ml-1 rounded p-0.5 text-gray-400 hover:bg-black/10 hover:text-gray-700 transition"
                  >
                    <svg
                      className="h-4 w-4"
                      viewBox="0 0 20 20"
                      fill="currentColor"
                      aria-hidden="true"
                    >
                      <path
                        fillRule="evenodd"
                        d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                        clipRule="evenodd"
                      />
                    </svg>
                  </button>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
