import {
  MatchStatus,
  matchStatusLabel,
  statusClass,
} from "../../enums/MatchStatus";
import type { BoardMatchDto } from "../../services/boardAccessService";
import type { ScoreDraft } from "./BoardAccessPage";

interface BoardAccessMatchProps {
  match: BoardMatchDto;
  index: number;
  busyMatchId: number | null;
  draft: ScoreDraft;
  editing: boolean;
  handleStart: (match: BoardMatchDto) => Promise<void>;
  handleFinish: (match: BoardMatchDto) => Promise<void>;
  setScoreDrafts: React.Dispatch<
    React.SetStateAction<Record<number, ScoreDraft>>
  >;
  setSelectedMatchId: React.Dispatch<React.SetStateAction<number | null>>;
}

export function BoardAccessMatch({
  match,
  index,
  busyMatchId,
  draft,
  editing,
  handleStart,
  handleFinish,
  setScoreDrafts,
  setSelectedMatchId,
}: BoardAccessMatchProps) {
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

  return (
    <section
      key={match.matchId}
      className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm"
    >
      <div className="border-b border-gray-100 px-5 py-4 sm:px-6">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.15em] text-gray-400">
              Spiel {index + 1} · {match.roundName || "Runde"}
            </p>
            <p className="mt-1 text-sm text-gray-500">
              {match.tournamentName} · {formatScheduledTime(match.plannedStart)}{" "}
              Uhr
            </p>
          </div>
          <span
            className={`rounded-full border px-3 py-1 text-xs font-semibold ${statusClass(match.status)}`}
          >
            {matchStatusLabel(match.status)}
          </span>
        </div>
      </div>

      <div className="px-5 py-6 sm:px-8">
        <div className="grid grid-cols-[1fr_auto_1fr] items-center gap-3 sm:gap-6">
          <div>
            <p
              className={`text-lg font-semibold sm:text-xl ${match.participants[0]?.isWinner ? "text-green-600" : "text-gray-900"}`}
            >
              {playerLabel(match, 0)}
            </p>
            <p className="mt-1 text-xs text-gray-400">Spieler 1</p>
          </div>
          <div className="rounded-lg bg-gray-50 px-4 py-2 text-2xl font-bold tracking-tight text-gray-900 sm:px-5 sm:text-3xl">
            {match.participants[0]?.score ?? 0} :{" "}
            {match.participants[1]?.score ?? 0}
          </div>
          <div className="text-right">
            <p
              className={`text-lg font-semibold sm:text-xl ${match.participants[1]?.isWinner ? "text-green-600" : "text-gray-900"}`}
            >
              {playerLabel(match, 1)}
            </p>
            <p className="mt-1 text-xs text-gray-400">Spieler 2</p>
          </div>
        </div>

        {match.status === MatchStatus.Scheduled && (
          <button
            type="button"
            onClick={() => void handleStart(match)}
            disabled={busyMatchId === match.matchId}
            className="mt-6 w-full rounded-lg bg-blue-600 px-4 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/30 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {busyMatchId === match.matchId
              ? "Wird gestartet..."
              : "▶ Spiel starten"}
          </button>
        )}

        {match.status === MatchStatus.InProgress && !editing && (
          <button
            type="button"
            onClick={() => setSelectedMatchId(match.matchId)}
            className="mt-6 w-full rounded-lg bg-blue-600 px-4 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/30"
          >
            Spiel beenden & Score eingeben
          </button>
        )}

        {match.status === MatchStatus.InProgress && editing && (
          <div className="mt-6 rounded-lg border border-gray-200 bg-gray-50 p-4 sm:p-5">
            <div className="mb-4">
              <h3 className="text-base font-semibold text-gray-900">
                Endergebnis eintragen
              </h3>
              <p className="mt-1 text-xs text-gray-500">
                Der Gewinner wird automatisch anhand des höheren Scores
                gespeichert.
              </p>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              {([0, 1] as const).map((playerIndex) => {
                const field = playerIndex === 0 ? "first" : "second";
                return (
                  <label
                    key={field}
                    htmlFor={`${field}-${match.matchId}`}
                    className="text-sm font-medium text-gray-700"
                  >
                    {playerLabel(match, playerIndex)}
                    <input
                      id={`${field}-${match.matchId}`}
                      inputMode="numeric"
                      pattern="[0-9]*"
                      min={0}
                      type="number"
                      value={draft[field]}
                      onChange={(event) =>
                        setScoreDrafts((current) => ({
                          ...current,
                          [match.matchId]: {
                            ...draft,
                            [field]: event.target.value,
                          },
                        }))
                      }
                      className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-3 text-2xl font-bold text-gray-900 shadow-sm outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                    />
                  </label>
                );
              })}
            </div>
            <div className="mt-5 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
              <button
                type="button"
                onClick={() => setSelectedMatchId(null)}
                className="rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-semibold text-gray-700 shadow-sm transition hover:bg-gray-50"
              >
                Abbrechen
              </button>
              <button
                type="button"
                onClick={() => void handleFinish(match)}
                disabled={busyMatchId === match.matchId}
                className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {busyMatchId === match.matchId
                  ? "Speichert..."
                  : "Ergebnis speichern"}
              </button>
            </div>
          </div>
        )}
      </div>
    </section>
  );
}
