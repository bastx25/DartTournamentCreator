import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router";
import Header from "../../components/Header";
import { DashboardHeader, type SavedTournamentItem } from "./DashboardHeader";
import { DashboardTournamentDetails } from "./DashboardTournamentDetails";
import { DashboardBraket } from "./DashboardBraket";
import { DashboardGroup } from "./DashboardGroup";
// import { DashboardBoards } from "./DashboardBoards";
import type { GroupDto } from "../../dtos/group/GroupDto";
import type { BraketDto } from "../../dtos/brakets/BraketDto";
import { getPublicTournamentByCode } from "../../services/publicTournamentService";
import {
  getSavedTournamentCodes,
  addTournamentCodeToCookie,
  removeTournamentCodeFromCookie,
} from "../../utils/cookieUtils";

export function DashboardPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialUrlCode = searchParams.get("code")?.trim().toUpperCase() || null;

  const [savedTournaments, setSavedTournaments] = useState<
    SavedTournamentItem[]
  >([]);
  const [activeCode, setActiveCode] = useState<string | null>(initialUrlCode);
  const [isInitializing, setIsInitializing] = useState(true);

  const [isAddingCode, setIsAddingCode] = useState(false);
  const [addCodeError, setAddCodeError] = useState<string | null>(null);

  const [selectedFilter, setSelectedFilter] = useState<string>("all");

  // Read saved codes from browser cookies on initial mount
  useEffect(() => {
    let isCancelled = false;

    async function initializeFromCookies() {
      try {
        setIsInitializing(true);
        const storedCodes = getSavedTournamentCodes();

        // If a code was passed in URL query param and not yet stored, include it
        const allCodesToLoad = [...storedCodes];
        if (initialUrlCode && !allCodesToLoad.includes(initialUrlCode)) {
          allCodesToLoad.unshift(initialUrlCode);
          addTournamentCodeToCookie(initialUrlCode);
        }

        if (allCodesToLoad.length === 0) {
          if (!isCancelled) {
            setSavedTournaments([]);
            setActiveCode(null);
          }
          return;
        }

        // Fetch fresh tournament data for each saved code
        const loadedItems: SavedTournamentItem[] = await Promise.all(
          allCodesToLoad.map(async (code): Promise<SavedTournamentItem> => {
            try {
              const data = await getPublicTournamentByCode(code);
              return { code, tournament: data, error: null };
            } catch {
              return {
                code,
                tournament: null,
                error: `Turnier mit Code "${code}" konnte nicht geladen werden oder existiert nicht mehr.`,
              };
            }
          }),
        );

        if (!isCancelled) {
          setSavedTournaments(loadedItems);

          // Determine initial active code
          const initialCandidate =
            (initialUrlCode &&
              loadedItems.find((i) => i.code === initialUrlCode && i.tournament)
                ?.code) ||
            loadedItems.find((i) => i.tournament)?.code ||
            (loadedItems.length > 0 ? loadedItems[0].code : null);

          setActiveCode(initialCandidate);
        }
      } finally {
        if (!isCancelled) {
          setIsInitializing(false);
        }
      }
    }

    void initializeFromCookies();

    return () => {
      isCancelled = true;
    };
  }, [initialUrlCode]);

  // Handle adding a tournament by code
  const handleAddTournamentCode = async (code: string): Promise<boolean> => {
    const normalized = code.trim().toUpperCase();
    if (!normalized) return false;

    // Check if already in current list
    if (savedTournaments.some((t) => t.code === normalized)) {
      setAddCodeError(
        `Das Turnier mit Code "${normalized}" wurde bereits hinzugefügt.`,
      );
      setActiveCode(normalized);
      setSearchParams({ code: normalized });
      return false;
    }

    try {
      setIsAddingCode(true);
      setAddCodeError(null);

      const tournament = await getPublicTournamentByCode(normalized);

      // Persist code in browser cookies
      addTournamentCodeToCookie(normalized);

      const newItem: SavedTournamentItem = {
        code: normalized,
        tournament,
        error: null,
      };

      setSavedTournaments((current) => [...current, newItem]);
      setActiveCode(normalized);
      setSelectedFilter("all");
      setSearchParams({ code: normalized });
      return true;
    } catch {
      setAddCodeError(`Kein Turnier mit dem Code "${normalized}" gefunden.`);
      return false;
    } finally {
      setIsAddingCode(false);
    }
  };

  // Handle removing a tournament from selection and cookies
  const handleRemoveTournament = (codeToRemove: string) => {
    // Remove from browser cookie
    removeTournamentCodeFromCookie(codeToRemove);

    // Remove from state
    const remaining = savedTournaments.filter((t) => t.code !== codeToRemove);
    setSavedTournaments(remaining);

    // If the removed tournament was active, switch active tournament
    if (activeCode === codeToRemove) {
      const nextActive =
        remaining.find((t) => t.tournament)?.code ||
        (remaining.length > 0 ? remaining[0].code : null);
      setActiveCode(nextActive);
      setSelectedFilter("all");
      if (nextActive) {
        setSearchParams({ code: nextActive });
      } else {
        setSearchParams({});
      }
    }
  };

  // Select an active tournament tab
  const handleSelectTournament = (code: string) => {
    setActiveCode(code);
    setSelectedFilter("all");
    setSearchParams({ code });
  };

  // Current active tournament data
  const currentItem = useMemo(() => {
    if (!activeCode) return null;
    return savedTournaments.find((t) => t.code === activeCode) || null;
  }, [savedTournaments, activeCode]);

  const activeTournament = currentItem?.tournament || null;

  const groups: GroupDto[] = useMemo(() => {
    return activeTournament?.groups ?? [];
  }, [activeTournament]);

  const brakets: BraketDto[] = useMemo(() => {
    return activeTournament?.brakets ?? [];
  }, [activeTournament]);

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

  const showGroupPlaceholder = selectedFilter === "all" && groups.length === 0;
  const showBraketPlaceholder =
    selectedFilter === "all" && brakets.length === 0;

  const braketsLength = brakets.length;
  const activeBraketCount = brakets.filter((b) => b.status === 1).length;
  const totalMatches = groups.length * 3 + brakets.length * 2;

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <main className="mx-auto w-full max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <DashboardHeader
          savedTournaments={savedTournaments}
          activeCode={activeCode}
          onSelectTournament={handleSelectTournament}
          onRemoveTournament={handleRemoveTournament}
          onAddTournamentCode={handleAddTournamentCode}
          isAddingCode={isAddingCode}
          addCodeError={addCodeError}
          setAddCodeError={setAddCodeError}
          selectedFilter={selectedFilter}
          onFilterChange={setSelectedFilter}
          groups={groups}
          brakets={brakets}
        />

        {isInitializing && (
          <div className="flex min-h-60 items-center justify-center">
            <div className="flex flex-col items-center gap-2">
              <div className="h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent" />
              <p className="text-sm text-gray-500">
                Lade gespeicherte Turniere...
              </p>
            </div>
          </div>
        )}

        {/* Initial Empty State (No tournaments in cookie) */}
        {!isInitializing && savedTournaments.length === 0 && (
          <section className="rounded-2xl border-2 border-dashed border-gray-300 bg-white p-10 text-center shadow-sm">
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-blue-50 text-3xl">
              🎯
            </div>
            <h2 className="mt-4 text-xl font-bold text-gray-900">
              Willkommen beim Dart Tournament Creator
            </h2>
            <p className="mx-auto mt-2 max-w-md text-sm text-gray-500 leading-relaxed">
              Es sind aktuell keine Turniere für dein Dashboard ausgewählt. Gib
              oben deinen <strong>Turnier-Code</strong> (z.&nbsp;B.{" "}
              <span className="font-mono text-gray-700">DART-XXXXXX</span>) ein,
              um ein Turnier hinzuzufügen und live zu verfolgen.
            </p>
          </section>
        )}

        {/* Unavailable Tournament Notice */}
        {!isInitializing && currentItem && currentItem.error && (
          <section className="rounded-xl border border-red-200 bg-red-50 p-6 text-center shadow-sm">
            <h2 className="text-base font-bold text-red-900">
              Turnier nicht verfügbar
            </h2>
            <p className="mt-1 text-sm text-red-700">{currentItem.error}</p>
            <button
              type="button"
              onClick={() => handleRemoveTournament(currentItem.code)}
              className="mt-4 inline-flex items-center gap-1.5 rounded-lg bg-red-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-red-700"
            >
              Turnier aus Dashboard entfernen
            </button>
          </section>
        )}

        {/* Active Tournament Content */}
        {!isInitializing && activeTournament && (
          <div className="space-y-6">
            <DashboardTournamentDetails
              tournament={activeTournament}
              braketsLength={braketsLength}
              totalMatches={totalMatches}
              activeBraketCount={activeBraketCount}
            />

            {/* Filter Empty State */}
            {selectedFilter !== "all" &&
              visibleGroups.length === 0 &&
              visibleBrakets.length === 0 && (
                <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
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
              <section className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center shadow-sm">
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
                {visibleGroups.map((group) => (
                  <DashboardGroup key={group.id} group={group} />
                ))}
              </div>
            )}

            {showBraketPlaceholder && (
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

            {visibleBrakets.length > 0 && (
              <div className="space-y-6">
                {visibleBrakets.map((braket) => (
                  <DashboardBraket key={braket.id} braket={braket} />
                ))}
              </div>
            )}
          </div>
        )}

        {/* Public Boards Section */}
        {/* <DashboardBoards /> */}
      </main>
    </div>
  );
}
