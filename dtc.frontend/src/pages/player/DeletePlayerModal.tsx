import type { Dto } from "../../dtos/player/PlayerDto";

interface DeletePlayerModalProps {
  player: Dto;
  deleting: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

export function DeletePlayerModal({
  player,
  deleting,
  onCancel,
  onConfirm,
}: DeletePlayerModalProps) {
  const playerName = player.displayName || `mit der Id ${player.id}`;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
      onClick={() => !deleting && onCancel()}
      role="presentation"
    >
      <div
        className="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl ring-1 ring-black/5"
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="delete-player-title"
        aria-describedby="delete-player-description"
      >
        <div className="p-6">
          {/* Icon */}
          <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-50">
            <svg
              className="h-6 w-6 text-red-600"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 9v3.75m-9.303 3.376L10.39 3.746a1.875 1.875 0 013.22 0l7.693 12.38A1.875 1.875 0 0119.693 19H4.307a1.875 1.875 0 01-1.61-2.874z"
              />
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 16.5h.008v.008H12V16.5z"
              />
            </svg>
          </div>

          <h3
            id="delete-player-title"
            className="mt-4 text-lg font-semibold text-gray-900"
          >
            Spieler löschen?
          </h3>

          <p
            id="delete-player-description"
            className="mt-2 text-sm leading-6 text-gray-500"
          >
            Möchtest du den Spieler{" "}
            <span className="font-semibold text-gray-900">{playerName}</span>{" "}
            wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.
          </p>
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-3 border-t border-gray-100 bg-gray-50 px-6 py-4">
          <button
            type="button"
            disabled={deleting}
            onClick={onCancel}
            className="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          >
            Abbrechen
          </button>

          <button
            type="button"
            disabled={deleting}
            onClick={onConfirm}
            className="inline-flex min-w-25 items-center justify-center gap-2 rounded-lg bg-red-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {deleting && (
              <svg
                className="h-4 w-4 animate-spin"
                viewBox="0 0 24 24"
                fill="none"
                aria-hidden="true"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                />
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
                />
              </svg>
            )}

            {deleting ? "Löschen..." : "Spieler löschen"}
          </button>
        </div>
      </div>
    </div>
  );
}
