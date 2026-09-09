import { useEffect, useState } from "react";
import Header from "../../components/Header";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";
import { DashboardHeader } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTournamentDetails";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import {
  getGroupsByTournamentId,
  getRoundsByTournamentId,
  getTournament,
} from "../../services/tournamentService";
import type { GroupDto } from "../../dtos/group/GroupDto";
import { DashboardRound } from "./DashboardRound";
import { DashboardGroup } from "./DashboardGroup";
import type { RoundDto } from "../../dtos/rounds/RoundDto";

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

  const totalMatches = 3;

  const activeRoundCount = 2;

  const roundsLength = 4;

  const dashboardError = tournamentsError;

  useEffect(() => {
    const SetInitialTournament = async () => {
      setSelectedTournamentId(tournaments[0].id);
    };

    if (tournaments.length > 0 && selectedTournamentId === null) {
      SetInitialTournament();
    }
  }, [tournaments, selectedTournamentId]);

  const [groups, setGroups] = useState<GroupDto[]>([]);
  useEffect(() => {
    if (tournament === null) return;

    const loadGroups = async () => {
      const response = await getGroupsByTournamentId(tournament?.id);
      setGroups(response);
    };

    loadGroups();
  }, [tournament]);

  const [rounds, setRounds] = useState<RoundDto[]>([]);

  useEffect(() => {
    if (tournament === null) return;

    const loadRounds = async () => {
      const response = await getRoundsByTournamentId(tournament.id);
      setRounds(response);
    };

    loadRounds();
  }, [tournament]);

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
              roundsLength={roundsLength}
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

            {rounds.length === 0 && (
              <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm mt-8">
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
                  return <DashboardRound key={round.id} round={round} />;
                })}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
