import { useCallback, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { MatchStatus } from "../../enums/MatchStatus";
import {
  finishBoardMatch,
  getBoardForAccess,
  getBoardMatches,
  startBoardMatch,
  type BoardMatchDto,
} from "../../services/boardAccessService";
import type { BoardDto } from "../../dtos/board/BoardDto";
import Header from "../../components/Header";
import { BoardAccessMatch } from "./BoardAccessMatch";

export interface ScoreDraft {
  first: string;
  second: string;
}

export function BoardAccessPage() {
  const { boardId } = useParams();
  const parsedBoardId = Number(boardId);
  const [board, setBoard] = useState<BoardDto | null>(null);
  const [matches, setMatches] = useState<BoardMatchDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [busyMatchId, setBusyMatchId] = useState<number | null>(null);
  const [selectedMatchId, setSelectedMatchId] = useState<number | null>(null);
  const [scoreDrafts, setScoreDrafts] = useState<Record<number, ScoreDraft>>(
    {},
  );
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!Number.isInteger(parsedBoardId) || parsedBoardId <= 0) {
      setError("Ungültige Board-ID.");
      setLoading(false);
      return;
    }

    try {
      const [boardData, matchData] = await Promise.all([
        getBoardForAccess(parsedBoardId),
        getBoardMatches(parsedBoardId),
      ]);
      setBoard(boardData);
      setMatches(matchData);
      setError(null);
      setScoreDrafts((current) => {
        const next = { ...current };
        for (const match of matchData) {
          if (match.status === MatchStatus.InProgress && !next[match.matchId]) {
            next[match.matchId] = {
              first: String(match.participants[0]?.score ?? 0),
              second: String(match.participants[1]?.score ?? 0),
            };
          }
        }
        return next;
      });
    } catch {
      setError(
        "Das Board oder die aktuellen Spiele konnten nicht geladen werden.",
      );
    } finally {
      setLoading(false);
    }
  }, [parsedBoardId]);

  useEffect(() => {
    let interval: number | undefined;

    const initialRefresh = async () => {
      await refresh();

      interval = window.setInterval(() => {
        void refresh();
      }, 5000);
    };

    const timer = window.setTimeout(() => {
      void initialRefresh();
    }, 0);

    return () => {
      window.clearTimeout(timer);

      if (interval !== undefined) {
        window.clearInterval(interval);
      }
    };
  }, [refresh]);

  const selectedMatch = useMemo(
    () => matches.find((match) => match.matchId === selectedMatchId) ?? null,
    [matches, selectedMatchId],
  );

  const handleStart = async (match: BoardMatchDto) => {
    setBusyMatchId(match.matchId);
    setError(null);
    try {
      const updated = await startBoardMatch(parsedBoardId, match.matchId);
      setMatches((current) =>
        current.map((item) =>
          item.matchId === updated.matchId ? updated : item,
        ),
      );
      setSelectedMatchId(updated.matchId);
      setScoreDrafts((current) => ({
        ...current,
        [updated.matchId]: {
          first: String(updated.participants[0]?.score ?? 0),
          second: String(updated.participants[1]?.score ?? 0),
        },
      }));
    } catch {
      setError("Das Spiel konnte nicht gestartet werden.");
    } finally {
      setBusyMatchId(null);
    }
  };

  const handleFinish = async (match: BoardMatchDto) => {
    const draft = scoreDrafts[match.matchId] ?? { first: "", second: "" };
    const first = Number(draft.first);
    const second = Number(draft.second);
    if (
      !Number.isInteger(first) ||
      !Number.isInteger(second) ||
      first < 0 ||
      second < 0
    ) {
      setError("Bitte zwei gültige Scores eingeben.");
      return;
    }
    if (first === second) {
      setError("Bitte einen eindeutigen Gewinner eingeben.");
      return;
    }

    setBusyMatchId(match.matchId);
    setError(null);
    try {
      const updated = await finishBoardMatch(parsedBoardId, match.matchId, {
        participants: [
          { participantId: match.participants[0].id, score: first },
          { participantId: match.participants[1].id, score: second },
        ],
      });
      setMatches((current) =>
        current.map((item) =>
          item.matchId === updated.matchId ? updated : item,
        ),
      );
      setSelectedMatchId(null);
    } catch (requestError) {
      console.error(requestError);
      setError("Das Ergebnis konnte nicht gespeichert werden.");
    } finally {
      setBusyMatchId(null);
    }
  };

  if (loading) {
    return (
      <main className="min-h-screen bg-gray-50 px-4 py-12 text-gray-900">
        <div className="mx-auto flex min-h-[60vh] max-w-4xl items-center justify-center">
          <div className="rounded-xl border border-gray-200 bg-white px-6 py-5 text-sm text-gray-500 shadow-sm">
            Board wird geladen...
          </div>
        </div>
      </main>
    );
  }

  const boardName = board?.label || `Board ${board?.number ?? parsedBoardId}`;

  return (
    <>
      <Header />

      <div className="min-h-screen bg-gray-50 text-gray-900">
        <main className="mx-auto w-full max-w-5xl px-4 py-8 sm:px-6 lg:px-8 lg:py-10">
          <header className="mb-8 flex flex-col gap-5 rounded-xl border border-gray-200 bg-white p-6 shadow-sm sm:flex-row sm:items-center sm:justify-between">
            <div>
              <div className="flex items-center gap-3">
                <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-red-600 text-xl shadow-sm">
                  🎯
                </div>
                <div>
                  <p className="text-xs font-semibold uppercase tracking-[0.2em] text-gray-400">
                    Landjugend Lasberg
                  </p>
                  <p className="text-xs font-medium text-gray-500">
                    Tournament Creator
                  </p>
                </div>
              </div>
              <div className="mt-6">
                <p className="text-sm font-medium text-gray-500">
                  Aktuelles Board
                </p>
                <h1 className="mt-1 text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">
                  {boardName}
                </h1>
                <p className="mt-2 text-sm text-gray-500">
                  Spiele starten, Ergebnisse eintragen und direkt am Board
                  speichern.
                </p>
              </div>
            </div>
            <button
              type="button"
              onClick={() => void refresh()}
              className="inline-flex items-center justify-center rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-semibold text-gray-700 shadow-sm transition hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
            >
              ↻ Aktualisieren
            </button>
          </header>

          {error && (
            <div className="mb-6 rounded-lg border border-red-500/20 bg-red-500/10 p-4 text-sm text-red-700">
              {error}
            </div>
          )}

          {matches.length === 0 ? (
            <section className="rounded-xl border border-dashed border-gray-300 bg-white p-10 text-center shadow-sm">
              <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-gray-100 text-2xl">
                🎯
              </div>
              <h2 className="mt-4 text-lg font-semibold text-gray-900">
                Keine Spiele auf diesem Board
              </h2>
              <p className="mx-auto mt-2 max-w-lg text-sm text-gray-500">
                Sobald ein Turnier für dieses Board angesetzt ist, erscheinen
                die Spiele hier automatisch.
              </p>
            </section>
          ) : (
            <div className="space-y-5">
              {matches.map((match, index) => {
                const draft = scoreDrafts[match.matchId] ?? {
                  first: String(match.participants[0]?.score ?? 0),
                  second: String(match.participants[1]?.score ?? 0),
                };
                const editing = selectedMatch?.matchId === match.matchId;

                return (
                  <BoardAccessMatch
                    key={index}
                    match={match}
                    index={index}
                    busyMatchId={busyMatchId}
                    draft={draft}
                    editing={editing}
                    handleStart={handleStart}
                    handleFinish={handleFinish}
                    setScoreDrafts={setScoreDrafts}
                    setSelectedMatchId={setSelectedMatchId}
                  />
                );
              })}
            </div>
          )}
        </main>
      </div>
    </>
  );
}

export default BoardAccessPage;
