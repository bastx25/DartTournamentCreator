import { useEffect, useState } from "react";
import { Link } from "react-router";
import { getPublicBoards } from "../../services/boardAccessService";
import type { PublicBoardDto } from "../../dtos/board/PublicBoardDto";

export function DashboardBoards() {
  const [boards, setBoards] = useState<PublicBoardDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isCancelled = false;

    async function loadBoards() {
      try {
        setLoading(true);
        setError(null);
        const data = await getPublicBoards();
        if (!isCancelled) {
          setBoards(data);
        }
      } catch (err) {
        if (!isCancelled) {
          console.error("Fehler beim Laden der öffentlichen Dartboards:", err);
          setError("Die Dartboards konnten aktuell nicht geladen werden.");
        }
      } finally {
        if (!isCancelled) {
          setLoading(false);
        }
      }
    }

    loadBoards();

    return () => {
      isCancelled = true;
    };
  }, []);

  return (
    <section className="mt-10 rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 border-b border-gray-100 pb-4">
        <div>
          <h2 className="text-xl font-bold tracking-tight text-gray-900 flex items-center gap-2">
            <span>🎯</span> Dartboards
          </h2>
          <p className="text-xs text-gray-500 mt-0.5">
            Öffentliche Übersicht aller Dartboards für Spieler, Zuschauer und Schiedsrichter.
          </p>
        </div>
        <span className="self-start sm:self-auto rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold text-blue-700 border border-blue-200">
          {boards.length} {boards.length === 1 ? "Board" : "Boards"} registriert
        </span>
      </div>

      {loading && (
        <div className="flex min-h-32 items-center justify-center">
          <p className="text-sm text-gray-400">Dartboards werden geladen...</p>
        </div>
      )}

      {error && (
        <div className="mt-4 rounded-lg bg-red-50 p-4 text-sm text-red-600 border border-red-200">
          {error}
        </div>
      )}

      {!loading && !error && boards.length === 0 && (
        <div className="py-8 text-center text-sm text-gray-500">
          Aktuell sind noch keine Dartboards angelegt.
        </div>
      )}

      {!loading && !error && boards.length > 0 && (
        <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          {boards.map((board) => {
            const displayName = board.label?.trim() || `Board #${board.id}`;

            return (
              <div
                key={board.id}
                className="group relative flex flex-col justify-between rounded-xl border border-gray-200 bg-gray-50/50 p-4 transition hover:border-blue-400 hover:bg-white hover:shadow-md"
              >
                <div>
                  <div className="flex items-start justify-between gap-2">
                    <h3 className="text-base font-bold text-gray-900 group-hover:text-blue-600 transition truncate" title={displayName}>
                      {displayName}
                    </h3>
                    <span
                      className={`inline-flex shrink-0 items-center rounded-full px-2 py-0.5 text-xs font-medium border ${
                        board.isActive
                          ? "bg-green-50 text-green-700 border-green-200"
                          : "bg-gray-100 text-gray-600 border-gray-200"
                      }`}
                    >
                      {board.isActive ? "Aktiv" : "Inaktiv"}
                    </span>
                  </div>

                  <div className="mt-2 text-xs text-gray-500 flex items-center gap-1.5">
                    <span>📍</span>
                    <span className="truncate">
                      {board.locationName || "Kein Austragungsort angegeben"}
                    </span>
                  </div>
                </div>

                <div className="mt-4 pt-3 border-t border-gray-200/60 flex items-center justify-between">
                  <span className="text-xs font-mono text-gray-400">
                    ID #{board.id}
                  </span>

                  <Link
                    to={`/boards/${board.id}`}
                    className="inline-flex items-center gap-1 rounded-lg bg-blue-600 px-3 py-1.5 text-xs font-semibold text-white shadow-sm transition hover:bg-blue-700 active:scale-95"
                  >
                    <span>🎯</span>
                    <span>Spielstand & Matches</span>
                  </Link>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </section>
  );
}
