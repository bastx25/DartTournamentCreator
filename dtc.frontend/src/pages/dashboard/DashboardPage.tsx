import { useEffect, useMemo, useState } from "react";
import Header from "../../components/Header";
import { RoundStatus } from "../../enums/RoundStatus";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";
import { DashboardHeader } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTournamentDetails";
import { DashboardGroup } from "./DashboardGroup";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import { getTournament } from "../../services/tournamentService";

export function DashboardPage() {
  const {
    tournaments,
    loading: tournamentsLoading,
    error: tournamentsError,
  } = useActiveTournaments();

  const [selectedTournamentId, setSelectedTournamentId] = useState<
    number | null
  >(null);

  const [tournament, setTournament] = useState<TournamentDto | null>(null);

  const effectiveTournamentId =
    selectedTournamentId !== null &&
    tournaments.some((tournament) => tournament.id === selectedTournamentId)
      ? selectedTournamentId
      : (tournaments[0]?.id ?? null);

  useEffect(() => {
    if (effectiveTournamentId === null) {
      return;
    }

    const loadSelectedTournament = async () => {
      const response = await getTournament(effectiveTournamentId);

      setTournament(response);
    };

    loadSelectedTournament();
  }, [effectiveTournamentId]);

  const rounds = useMemo(
    () =>
      [...(tournament?.rounds ?? [])].sort(
        (a, b) =>
          new Date(a.plannedStart).getTime() -
          new Date(b.plannedStart).getTime(),
      ),
    [tournament],
  );

  const groups = tournament?.groups ?? [];

  const totalMatches = useMemo(
    () =>
      rounds.reduce((total, round) => total + (round.matches?.length ?? 0), 0),
    [rounds],
  );

  const activeRoundCount = useMemo(
    () =>
      rounds.filter((round) => round.status === RoundStatus.InProgress).length,
    [rounds],
  );

  const dashboardError = tournamentsError;

  useEffect(() => {
    const SetInitialTournament = async () => {
      setSelectedTournamentId(tournaments[0].id);
    };

    if (tournaments.length > 0 && selectedTournamentId === null) {
      SetInitialTournament();
    }
  }, [tournaments, selectedTournamentId]);

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

            {groups.length === 0 && (
              <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
                <h2 className="text-lg font-semibold text-gray-900">
                  Noch keine Gruppen vorhanden
                </h2>
                <p className="mt-2 text-sm text-gray-500">
                  Für dieses Turnier wurden noch keine Gruppen erstellt.
                </p>
              </section>
            )}

            {groups.length > 0 && (
              <div className="space-y-6">
                {groups.map((group) => {
                  return <DashboardGroup key={group.id} group={group} />;
                })}
              </div>
            )}

            {/* {rounds.length === 0 && (
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
                      key={round.id}
                      round={round}
                      sortedMatches={sortedMatches}
                      playerName={playerName}
                    />
                  );
                })}
              </div>
            )} */}
          </>
        )}
      </main>
    </div>
  );
}
