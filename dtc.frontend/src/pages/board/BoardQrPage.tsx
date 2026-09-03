import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import type { BoardDto } from "../../dtos/board/BoardDto";
import { getBoardForAccess } from "../../services/boardAccessService";

function getPublicBaseUrl() {
  const configured = import.meta.env.VITE_PUBLIC_APP_URL as string | undefined;
  return (configured?.trim() || window.location.origin).replace(/\/$/, "");
}

export function BoardQrPage() {
  const { boardId } = useParams();
  const parsedBoardId = Number(boardId);
  const [board, setBoard] = useState<BoardDto | null>(null);

  const accessUrl = useMemo(
    () => `${getPublicBaseUrl()}/boards/${parsedBoardId}`,
    [parsedBoardId],
  );
  const qrUrl = useMemo(
    () =>
      `https://quickchart.io/qr?size=420&margin=2&text=${encodeURIComponent(accessUrl)}`,
    [accessUrl],
  );

  useEffect(() => {
    if (Number.isInteger(parsedBoardId) && parsedBoardId > 0) {
      void getBoardForAccess(parsedBoardId)
        .then(setBoard)
        .catch(() => setBoard(null));
    }
  }, [parsedBoardId]);

  const boardName = board?.label || `${board?.number ?? parsedBoardId}`;

  return (
    <div className="min-h-screen bg-gray-50 text-gray-900 print:bg-white">
      <main className="mx-auto w-full max-w-3xl px-4 py-8 sm:px-6 lg:px-8 lg:py-10">
        <header className="mb-8 flex flex-col gap-5 rounded-xl border border-gray-200 bg-white p-6 shadow-sm sm:flex-row sm:items-center sm:justify-between print:hidden">
          <div className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-red-600 text-xl shadow-sm">
              🎯
            </div>
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.2em] text-gray-400">
                Landjugend Lasberg
              </p>
              <h1 className="mt-1 text-xl font-bold tracking-tight text-gray-900">
                Board QR-Code
              </h1>
            </div>
          </div>
          <button
            type="button"
            onClick={() => window.print()}
            className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/30"
          >
            QR drucken
          </button>
        </header>

        <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm print:border-0 print:shadow-none">
          <div className="border-b border-gray-100 px-6 py-5 text-center sm:px-10">
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-gray-400">
              Board-Zugang
            </p>
            <h2 className="mt-2 text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">
              Board {boardName}
            </h2>
            <p className="mx-auto mt-2 max-w-md text-sm text-gray-500">
              QR-Code scannen, um die Spiele dieses Boards aufzurufen und
              Ergebnisse einzutragen.
            </p>
          </div>

          <div className="px-6 py-8 text-center sm:px-10 sm:py-10">
            <div className="mx-auto w-fit rounded-2xl border border-gray-200 bg-white p-4 shadow-sm sm:p-5">
              <img
                src={qrUrl}
                alt={`QR-Code für ${boardName}`}
                className="h-auto w-full max-w-[420px]"
              />
            </div>

            <div className="mx-auto mt-7 max-w-xl rounded-lg border border-gray-200 bg-gray-50 p-4">
              <p className="text-xs font-semibold uppercase tracking-[0.15em] text-gray-400">
                Scan-Ziel
              </p>
              <p className="mt-2 break-all text-sm font-medium text-gray-700">
                {accessUrl}
              </p>
            </div>

            <div className="mt-8 grid gap-3 text-left sm:grid-cols-3">
              <div className="rounded-lg border border-gray-200 bg-white p-4">
                <p className="text-sm font-semibold text-gray-900">
                  1. Scannen
                </p>
                <p className="mt-1 text-xs leading-5 text-gray-500">
                  QR-Code mit dem Smartphone öffnen.
                </p>
              </div>
              <div className="rounded-lg border border-gray-200 bg-white p-4">
                <p className="text-sm font-semibold text-gray-900">
                  2. Spielen
                </p>
                <p className="mt-1 text-xs leading-5 text-gray-500">
                  Das zugehörige Spiel auswählen und starten.
                </p>
              </div>
              <div className="rounded-lg border border-gray-200 bg-white p-4">
                <p className="text-sm font-semibold text-gray-900">
                  3. Ergebnis
                </p>
                <p className="mt-1 text-xs leading-5 text-gray-500">
                  Score eintragen und Ergebnis speichern.
                </p>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  );
}

export default BoardQrPage;
