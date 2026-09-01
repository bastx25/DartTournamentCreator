import type { Player } from "../types/Player";

interface DeletePlayerModalProps {
  player: Player;
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
  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4 backdrop-blur-sm"
      onClick={() => !deleting && onCancel()}
    >
      <div
        className="w-full max-w-md rounded-xl border border-gray-200 bg-white p-6 shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <h3 className="text-lg font-semibold text-gray-900">
          Spieler löschen?
        </h3>

        <p className="mt-2 text-sm text-gray-500">
          Möchtest du{" "}
          <span className="font-medium text-gray-900">
            {player.nickname || player.displayName}
          </span>{" "}
          wirklich löschen?
        </p>

        <div className="mt-6 flex justify-end gap-3">
          <button type="button" disabled={deleting} onClick={onCancel}>
            Abbrechen
          </button>

          <button type="button" disabled={deleting} onClick={onConfirm}>
            {deleting ? "Löschen..." : "Löschen"}
          </button>
        </div>
      </div>
    </div>
  );
}
