import { useCallback, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { MatchStatus, matchStatusLabel } from "../../enums/MatchStatus";
import {
  finishBoardMatch,
  getBoardForAccess,
  getBoardMatches,
  startBoardMatch,
  type BoardMatchDto,
} from "../../services/boardAccessService";
import type { BoardDto } from "../../dtos/board/BoardDto";

function playerLabel(match: BoardMatchDto, index: number) {
  return match.participants[index]?.player?.displayName ?? "TBD";
}

function formatScheduledTime(value: string | null) {
  if (!value) return "Zeit offen";
  return new Intl.DateTimeFormat("de-DE", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value));
}

function statusClass(status: MatchStatus) {
  switch (status) {
    case MatchStatus.InProgress:
      return "border-amber-200 bg-amber-50 text-amber-700";
    case MatchStatus.Completed:
      return "border-green-200 bg-green-50 text-green-700";
    case MatchStatus.Cancelled:
      return "border-red-200 bg-red-50 text-red-700";
    default:
      return "border-gray-200 bg-gray-50 text-gray-600";
  }
}

interface ScoreDraft {
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
  const [scoreDrafts, setScoreDrafts] = useState<Record<number, ScoreDraft>>({});
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
      setError("Das Board oder die aktuellen Spiele konnten nicht geladen werden.");
    } finally {
      setLoading(false);
    }
  }, [parsedBoardId]);

  useEffect(() => {
    void refresh();
    const timer = window.setInterval(() => void refresh(), 5000);
    return () => window.clearInterval(timer);
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
      setMatches((current) => current.map((item) => item.matchId === updated.matchId ? updated : item));
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
    if (!Number.isInteger(first) || !Number.isInteger(second) || first < 0 || second < 0) {
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
      setMatches((current) => current.map((item) => item.matchId === updated.matchId ? updated : item));
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
    <div className="min-h-screen bg-gray-50 text-gray-900">
      <main className="mx-auto w-full max-w-5xl px-4 py-8 sm:px-6 lg:px-8 lg:py-10">
        <header className="mb-8 flex flex-col gap-5 rounded-xl border border-gray-200 bg-white p-6 shadow-sm sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex items-center gap-3">
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-red-600 text-xl shadow-sm">🎯</div>
              <div>
                <p className="text-xs font-semibold uppercase tracking-[0.2em] text-gray-400">Landjugend Lasberg</p>
                <p className="text-xs font-medium text-gray-500">Tournament Creator</p>
              </div>
            </div>
            <div className="mt-6">
              <p className="text-sm font-medium text-gray-500">Aktuelles Board</p>
              <h1 className="mt-1 text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">{boardName}</h1>
              <p className="mt-2 text-sm text-gray-500">Spiele starten, Ergebnisse eintragen und direkt am Board speichern.</p>
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
          <div className="mb-6 rounded-lg border border-red-500/20 bg-red-500/10 p-4 text-sm text-red-700">{error}</div>
        )}

        {matches.length === 0 ? (
          <section className="rounded-xl border border-dashed border-gray-300 bg-white p-10 text-center shadow-sm">
            <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-gray-100 text-2xl">🎯</div>
            <h2 className="mt-4 text-lg font-semibold text-gray-900">Keine Spiele auf diesem Board</h2>
            <p className="mx-auto mt-2 max-w-lg text-sm text-gray-500">Sobald ein Turnier für dieses Board angesetzt ist, erscheinen die Spiele hier automatisch.</p>
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
                <section key={match.matchId} className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
                  <div className="border-b border-gray-100 px-5 py-4 sm:px-6">
                    <div className="flex flex-wrap items-center justify-between gap-3">
                      <div>
                        <p className="text-xs font-semibold uppercase tracking-[0.15em] text-gray-400">Spiel {index + 1} · {match.roundName || "Runde"}</p>
                        <p className="mt-1 text-sm text-gray-500">{match.tournamentName} · {formatScheduledTime(match.plannedStart)} Uhr</p>
                      </div>
                      <span className={`rounded-full border px-3 py-1 text-xs font-semibold ${statusClass(match.status)}`}>{matchStatusLabel(match.status)}</span>
                    </div>
                  </div>

                  <div className="px-5 py-6 sm:px-8">
                    <div className="grid grid-cols-[1fr_auto_1fr] items-center gap-3 sm:gap-6">
                      <div>
                        <p className={`text-lg font-semibold sm:text-xl ${match.participants[0]?.isWinner ? "text-green-600" : "text-gray-900"}`}>{playerLabel(match, 0)}</p>
                        <p className="mt-1 text-xs text-gray-400">Spieler 1</p>
                      </div>
                      <div className="rounded-lg bg-gray-50 px-4 py-2 text-2xl font-bold tracking-tight text-gray-900 sm:px-5 sm:text-3xl">
                        {match.participants[0]?.score ?? 0} : {match.participants[1]?.score ?? 0}
                      </div>
                      <div className="text-right">
                        <p className={`text-lg font-semibold sm:text-xl ${match.participants[1]?.isWinner ? "text-green-600" : "text-gray-900"}`}>{playerLabel(match, 1)}</p>
                        <p className="mt-1 text-xs text-gray-400">Spieler 2</p>
                      </div>
                    </div>

                    {match.status === MatchStatus.Scheduled && (
                      <button type="button" onClick={() => void handleStart(match)} disabled={busyMatchId === match.matchId} className="mt-6 w-full rounded-lg bg-blue-600 px-4 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/30 disabled:cursor-not-allowed disabled:opacity-60">
                        {busyMatchId === match.matchId ? "Wird gestartet..." : "▶ Spiel starten"}
                      </button>
                    )}

                    {match.status === MatchStatus.InProgress && !editing && (
                      <button type="button" onClick={() => setSelectedMatchId(match.matchId)} className="mt-6 w-full rounded-lg bg-blue-600 px-4 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/30">
                        Spiel beenden & Score eingeben
                      </button>
                    )}

                    {match.status === MatchStatus.InProgress && editing && (
                      <div className="mt-6 rounded-lg border border-gray-200 bg-gray-50 p-4 sm:p-5">
                        <div className="mb-4">
                          <h3 className="text-base font-semibold text-gray-900">Endergebnis eintragen</h3>
                          <p className="mt-1 text-xs text-gray-500">Der Gewinner wird automatisch anhand des höheren Scores gespeichert.</p>
                        </div>
                        <div className="grid gap-4 sm:grid-cols-2">
                          {([0, 1] as const).map((playerIndex) => {
                            const field = playerIndex === 0 ? "first" : "second";
                            return (
                              <label key={field} htmlFor={`${field}-${match.matchId}`} className="text-sm font-medium text-gray-700">
                                {playerLabel(match, playerIndex)}
                                <input id={`${field}-${match.matchId}`} inputMode="numeric" pattern="[0-9]*" min={0} type="number" value={draft[field]} onChange={(event) => setScoreDrafts((current) => ({ ...current, [match.matchId]: { ...draft, [field]: event.target.value } }))} className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-3 text-2xl font-bold text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20" />
                              </label>
                            );
                          })}
                        </div>
                        <div className="mt-5 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
                          <button type="button" onClick={() => setSelectedMatchId(null)} className="rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-semibold text-gray-700 shadow-sm transition hover:bg-gray-50">Abbrechen</button>
                          <button type="button" onClick={() => void handleFinish(match)} disabled={busyMatchId === match.matchId} className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60">
                            {busyMatchId === match.matchId ? "Speichert..." : "Ergebnis speichern"}
                          </button>
                        </div>
                      </div>
                    )}
                  </div>
                </section>
              );
            })}
          </div>
        )}
      </main>
    </div>
  );
}

export default BoardAccessPage;
