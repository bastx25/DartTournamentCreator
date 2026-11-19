import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router";
import { useActiveTournaments } from "../../hooks/useActiveTournaments";
import { DashboardHeader } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTournamentDetails";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import type { PublicTournamentDto } from "../../dtos/public/PublicTournamentDto";
import {
  getPublicTournamentByCode,
  getPublicTournamentById,
} from "../../services/publicTournamentService";
import type { GroupDto } from "../../dtos/group/GroupDto";
import { DashboardBraket } from "./DashboardBraket";
import { DashboardGroup } from "./DashboardGroup";
import type { BraketDto } from "../../dtos/brakets/BraketDto";
import Header from "../../components/Header";

export function DashboardPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialCode = searchParams.get("code")?.trim().toUpperCase() || null;

  const {
    tournaments,
    loading: tournamentsLoading,
    error: tournamentsError,
  } = useActiveTournaments();

  const [selectedTournamentId, setSelectedTournamentId] = useState<
    number | null
  >(null);

  const [activeCode, setActiveCode] = useState<string | null>(initialCode);
  const [tournamentCodeQuery, setTournamentCodeQuery] = useState(
    initialCode || "",
  );
  const [isCodeSearching, setIsCodeSearching] = useState(false);
  const [codeError, setCodeError] = useState<string | null>(null);

  const [tournament, setTournament] = useState<
    TournamentDto | PublicTournamentDto | null
  >(null);
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [brakets, setBrakets] = useState<BraketDto[]>([]);
  const [selectedFilter, setSelectedFilter] = useState<string>("all");
  const [tournamentDetailsLoading, setTournamentDetailsLoading] =
    useState(false);

  const effectiveTournamentId =
    selectedTournamentId !== null &&
    tournaments.some((t) => t.id === selectedTournamentId)
      ? selectedTournamentId
      : (tournaments[0]?.id ?? null);

  // Set default initial active tournament if no code is specified
  useEffect(() => {
    if (
      !activeCode &&
      tournaments.length > 0 &&
      selectedTournamentId === null
    ) {
      setSelectedTournamentId(tournaments[0].id);
    }
  }, [tournaments, selectedTournamentId, activeCode]);

  // Reset filter when switching tournaments
  useEffect(() => {
    setSelectedFilter("all");
  }, [effectiveTournamentId, activeCode]);

  // Load tournament by code if activeCode is set
  useEffect(() => {
    if (!activeCode) return;

    let isCancelled = false;

    const loadByCode = async () => {
      try {
        setIsCodeSearching(true);
        setTournamentDetailsLoading(true);
        setCodeError(null);

        const data = await getPublicTournamentByCode(activeCode);
        if (isCancelled) return;

        setTournament(data);
        setGroups(data.groups ?? []);
        setBrakets(data.brakets ?? []);
      } catch (err) {
        if (isCancelled) return;
        console.error("Fehler beim Laden des Turniers per Code:", err);
        setCodeError(`Kein Turnier mit dem Code "${activeCode}" gefunden.`);
        setTournament(null);
        setGroups([]);
        setBrakets([]);
      } finally {
        if (!isCancelled) {
          setIsCodeSearching(false);
          setTournamentDetailsLoading(false);
        }
      }
    };

    void loadByCode();

    return () => {
      isCancelled = true;
    };
  }, [activeCode]);

  // Load tournament by ID if no activeCode is set
  useEffect(() => {
    if (activeCode || effectiveTournamentId === null) return;

    let isCancelled = false;

    const loadById = async () => {
      try {
        setTournamentDetailsLoading(true);
        setCodeError(null);

        const data = await getPublicTournamentById(effectiveTournamentId);
        if (isCancelled) return;

        setTournament(data);
        setGroups(data.groups ?? []);
        setBrakets(data.brakets ?? []);
      } catch (err) {
        if (isCancelled) return;
        console.error("Fehler beim Laden des Turniers:", err);
        setTournament(null);
        setGroups([]);
        setBrakets([]);
      } finally {
        if (!isCancelled) {
          setTournamentDetailsLoading(false);
        }
      }
    };

    void loadById();

    return () => {
      isCancelled = true;
    };
  }, [activeCode, effectiveTournamentId]);

  const handleSearchCodeSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = tournamentCodeQuery.trim().toUpperCase();
    if (!trimmed) return;

    setActiveCode(trimmed);
    setSearchParams({ code: trimmed });
  };

  const handleClearCode = () => {
    setActiveCode(null);
    setTournamentCodeQuery("");
    setCodeError(null);
    setSearchParams({});
    if (tournaments.length > 0) {
      setSelectedTournamentId(tournaments[0].id);
    }
  };

  const visibleGroups = useMemo(() => {
    if (selectedFilter === "all" || selectedFilter === "group-all") {
      return groups;
    }
    if (selectedFilter.startsWith("group-")) {
      const groupId = Number(selectedFilter.replace("group-", ""));
      return groups.filter((g) => g.id === groupId);
    }
    return [];
  }, [groups, selectedFilter]);

  const visibleBrakets = useMemo(() => {
    if (selectedFilter === "all" || selectedFilter === "knockout-all") {
      return brakets;
    }
    if (selectedFilter.startsWith("braket-")) {
      const braketId = Number(selectedFilter.replace("braket-", ""));
      return brakets.filter((b) => b.id === braketId);
    }
    return [];
  }, [brakets, selectedFilter]);

  const showGroupPlaceholder =
    selectedFilter === "all" && groups.length === 0;
  const showBraketPlaceholder =
    selectedFilter === "all" && brakets.length === 0;

  const braketsLength = brakets.length;
  const activeBraketCount = brakets.filter((b) => b.status === 1).length;
  const totalMatches = groups.length * 3 + brakets.length * 2;

  const dashboardError = codeError || tournamentsError;

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <main className="mx-auto w-full max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <DashboardHeader
          effectiveTournamentId={effectiveTournamentId}
          setSelectedTournamentId={setSelectedTournamentId}
          tournamentsLoading={tournamentsLoading}
          tournaments={tournaments}
          selectedFilter={selectedFilter}
          onFilterChange={setSelectedFilter}
          groups={groups}
          brakets={brakets}
          tournamentCodeQuery={tournamentCodeQuery}
          onTournamentCodeQueryChange={setTournamentCodeQuery}
          onSearchCodeSubmit={handleSearchCodeSubmit}
          activeCode={activeCode}
          onClearCode={handleClearCode}
          isCodeSearching={isCodeSearching}
        />

        {dashboardError && (
          <div className="mb-6 rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm font-medium text-red-600">{dashboardError}</p>
          </div>
        )}

        {(tournamentsLoading || tournamentDetailsLoading) && (
          <div className="flex min-h-75 items-center justify-center">
            <div className="flex flex-col items-center gap-2">
              <div className="h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent" />
              <p className="text-sm text-gray-400">
                Turnierdaten werden geladen...
              </p>
            </div>
          </div>
        )}

        {!tournamentsLoading &&
          !tournamentDetailsLoading &&
          !activeCode &&
          tournaments.length === 0 &&
          !dashboardError && (
            <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
              <h2 className="text-lg font-semibold text-gray-900">
                Keine aktiven Turniere
              </h2>
              <p className="mt-2 text-sm text-gray-500">
                Aktuell sind keine öffentlichen Turniere aktiv. Du kannst oben
                einen Turnier-Code eingeben oder dich anmelden, um ein neues
                Turnier zu erstellen.
              </p>
            </section>
          )}

        {!tournamentDetailsLoading && tournament && (
          <>
            <DashboardTournamentDetails
              tournament={tournament}
              braketsLength={braketsLength}
              totalMatches={totalMatches}
              activeBraketCount={activeBraketCount}
            />

            {/* Filter Empty State */}
            {selectedFilter !== "all" &&
              visibleGroups.length === 0 &&
              visibleBrakets.length === 0 && (
                <section className="mt-6 rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
                  <h2 className="text-lg font-semibold text-gray-900">
                    Keine Matches für den ausgewählten Filter
                  </h2>
                  <p className="mt-2 text-sm text-gray-500">
                    In diesem Bereich sind noch keine Matches oder Gruppen
                    vorhanden.
                  </p>
                </section>
              )}

            {showGroupPlaceholder && (
              <section className="mb-6 rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
                <h2 className="text-lg font-semibold text-gray-900">
                  Noch keine Gruppen vorhanden
                </h2>
                <p className="mt-2 text-sm text-gray-500">
                  Für dieses Turnier wurden noch keine Gruppen erstellt.
                </p>
              </section>
            )}

            {visibleGroups.length > 0 && (
              <div className="space-y-6">
                {visibleGroups.map((group) => {
                  return <DashboardGroup key={group.id} group={group} />;
                })}
              </div>
            )}

            {showBraketPlaceholder && (
              <section className="mt-8 rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
                <h2 className="text-lg font-semibold text-gray-900">
                  Noch keine Runden vorhanden
                </h2>
                <p className="mt-2 text-sm text-gray-500">
                  Für dieses Turnier wurden noch keine Runden und Matches
                  erstellt.
                </p>
              </section>
            )}

            {visibleBrakets.length > 0 && (
              <div className="mt-8 space-y-6">
                {visibleBrakets.map((braket) => {
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
