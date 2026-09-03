import { useMemo, useState } from "react";
import Header from "../../components/Header";
import type { RoundDto } from "../../dtos/Round/RoundDto";
import { RoundStatus } from "../../enums/RoundStatus";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";
import { DashboardHeader } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTorunamentDetails";
import { DashboardRound } from "./DashboardRound";

export type MatchSchedule = RoundDto["matches"][number];

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
        <DashboardHeader
          effectiveTournamentId={effectiveTournamentId}
          setSelectedTournamentId={setSelectedTournamentId}
          tournamentsLoading={tournamentsLoading}
          tournaments={tournaments}
        />
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
            <DashboardTournamentDetails
              tournament={tournament}
              roundsLength={rounds.length}
              totalMatches={totalMatches}
              activeRoundCount={activeRoundCount}
            />

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
                    <DashboardRound
                      round={round}
                      sortedMatches={sortedMatches}
                      playerName={playerName}
                      matchScore={matchScore}
                      matchTime={matchTime}
                    />
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
