import { useEffect, useState } from "react";
import Header from "../../components/Header";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";
import { DashboardHeader } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTournamentDetails";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import {
  getGroupsByTournamentId,
  getBraketsByTournamentId,
  getTournament,
} from "../../services/tournamentService";
import type { GroupDto } from "../../dtos/group/GroupDto";
import { DashboardBraket } from "./DashboardBraket";
import { DashboardGroup } from "./DashboardGroup";
import type { BraketDto } from "../../dtos/brakets/BraketDto";

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

  const activeBraketCount = 2;

  const braketsLength = 4;

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

  const [brakets, setBrakets] = useState<BraketDto[]>([]);

  useEffect(() => {
    if (tournament === null) return;

    const loadBrakets = async () => {
      const response = await getBraketsByTournamentId(tournament.id);
      setBrakets(response);
    };

    loadBrakets();
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
          <div className="mb-6 braketed-lg border border-red-500/20 bg-red-500/10 p-4">
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
          <section className="braketed-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
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
              braketsLength={braketsLength}
              totalMatches={totalMatches}
              activeBraketCount={activeBraketCount}
            />

            {groups.length === 0 && (
              <section className="braketed-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
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

            {brakets.length === 0 && (
              <section className="braketed-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm mt-8">
                <h2 className="text-lg font-semibold text-gray-900">
                  Noch keine Runden vorhanden
                </h2>
                <p className="mt-2 text-sm text-gray-500">
                  Für dieses Turnier wurden noch keine Runden und Matches
                  erstellt.
                </p>
              </section>
            )}

            {brakets.length > 0 && (
              <div className="space-y-6 mt-8">
                {brakets.map((braket) => {
                  return <DashboardBraket key={braket.id} braket={braket} />;
                })}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
