import { useMemo, useState } from "react";
import Header from "../../components/Header";

import type { RoundDto } from "../../dtos/Round/RoundDto";
import { TournamentStatus } from "../../enums/TournamentStatus";
import { RoundStatus } from "../../enums/RoundStatus";
import { MatchStatus } from "../../enums/MatchStatus";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";

type MatchSchedule = RoundDto["matches"][number];

const dateFormatter = new Intl.DateTimeFormat("de-DE", {
  dateStyle: "medium",
  timeStyle: "short",
});

const timeFormatter = new Intl.DateTimeFormat("de-DE", {
  hour: "2-digit",
  minute: "2-digit",
});

function formatDate(value: string) {
  return dateFormatter.format(new Date(value));
}

function formatTime(value: string) {
  return timeFormatter.format(new Date(value));
}

function tournamentStatusLabel(status: TournamentStatus): string {
  switch (status) {
    case TournamentStatus.Scheduled:
      return "Geplant";
    case TournamentStatus.InProgress:
      return "Laufend";
    default:
      return "Aktiv";
  }
}

function roundStatusLabel(status: RoundStatus) {
  switch (status) {
    case RoundStatus.InProgress:
      return "Läuft";
    case RoundStatus.Completed:
      return "Abgeschlossen";
    default:
      return "Geplant";
  }
}

function matchStatusLabel(status: MatchStatus) {
  switch (status) {
    case MatchStatus.InProgress:
      return "Läuft";
    case MatchStatus.Completed:
      return "Beendet";
    case MatchStatus.Cancelled:
      return "Abgesagt";
    default:
      return "Geplant";
  }
}

function matchTime(match: MatchSchedule, roundStart: string) {
  return match.actualStart ?? roundStart;
}

function playerName(match: MatchSchedule, index: number) {
  const player = match.participants[index]?.player;
  return player?.displayName ?? "TBD";
}

function matchScore(match: MatchSchedule, index: number) {
  return match.participants[index]?.score ?? null;
}

export function DashboardPage() {
  const {
    tournaments,
    loading: tournamentsLoading,
    error: tournamentsError,
  } = useActiveTournaments();

  const [selectedTournamentId, setSelectedTournamentId] = useState<
    number | null
  >(null);

  const effectiveTournamentId =
    selectedTournamentId !== null &&
    tournaments.some((tournament) => tournament.id === selectedTournamentId)
      ? selectedTournamentId
      : (tournaments[0]?.id ?? null);

  const tournament = useMemo(
    () => tournaments.find((item) => item.id === selectedTournamentId) ?? null,
    [selectedTournamentId, tournaments],
  );

  const rounds = useMemo(
    () =>
      [...(tournament?.rounds ?? [])].sort(
        (a, b) =>
          new Date(a.plannedStart).getTime() -
          new Date(b.plannedStart).getTime(),
      ),
    [tournament],
  );

  const totalMatches = useMemo(
    () => rounds.reduce((total, round) => total + round.matches.length, 0),
    [rounds],
  );

  const activeRoundCount = useMemo(
    () =>
      rounds.filter((round) => round.status === RoundStatus.InProgress).length,
    [rounds],
  );

  const dashboardError = tournamentsError;

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <main className="mx-auto w-full max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
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

        {dashboardError && (
          <div className="mb-6 rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-600">{dashboardError}</p>
          </div>
        )}

        {tournamentsLoading && (
          <div className="flex min-h-75 items-center justify-center">
            <p className="text-sm text-gray-400">
              Aktive Turniere werden geladen...
            </p>
          </div>
        )}

        {!tournamentsLoading && tournaments.length === 0 && !dashboardError && (
          <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
            <h2 className="text-lg font-semibold text-gray-900">
              Keine aktiven Turniere
            </h2>
            <p className="mt-2 text-sm text-gray-500">
              Sobald ein Turnier geplant oder laufend ist, erscheint es hier.
            </p>
          </section>
        )}

        {!tournamentsLoading && tournament && (
          <>
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
                      {tournament.description
                        ? ` · ${tournament.description}`
                        : ""}
                    </p>
                  </div>

                  <dl className="grid grid-cols-3 gap-4 text-sm sm:min-w-96">
                    <div>
                      <dt className="text-gray-500">Runden</dt>
                      <dd className="mt-1 text-lg font-semibold text-gray-900">
                        {rounds.length}
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

            {rounds.length === 0 && (
              <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
                <h2 className="text-lg font-semibold text-gray-900">
                  Noch keine Runden vorhanden
                </h2>
                <p className="mt-2 text-sm text-gray-500">
                  Für dieses Turnier wurden noch keine Runden und Matches
                  erstellt.
                </p>
              </section>
            )}

            {rounds.length > 0 && (
              <div className="space-y-6">
                {rounds.map((round) => {
                  const sortedMatches = [...round.matches].sort(
                    (a, b) =>
                      new Date(matchTime(a, round.plannedStart)).getTime() -
                      new Date(matchTime(b, round.plannedStart)).getTime(),
                  );

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
                          const scheduledTime = matchTime(
                            match,
                            round.plannedStart,
                          );
                          const firstScore = matchScore(match, 0);
                          const secondScore = matchScore(match, 1);

                          return (
                            <div
                              key={match.id}
                              className="grid gap-4 px-6 py-4 sm:grid-cols-[110px_minmax(0,1fr)_120px] sm:items-center sm:px-8"
                            >
                              <div>
                                <p className="text-sm font-semibold text-gray-900">
                                  Match {index + 1}
                                </p>
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
                                      <p className="mt-1 text-xs text-gray-500">
                                        Score: {firstScore}
                                      </p>
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
                                      <p className="mt-1 text-xs text-gray-500">
                                        Score: {secondScore}
                                      </p>
                                    )}
                                  </div>
                                </div>
                              </div>

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
                        })}
                      </div>
                    </section>
                  );
                })}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
