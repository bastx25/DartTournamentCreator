import { Link } from "react-router";
import type { BoardDto } from "../../dtos/board/BoardDto";

interface BoardRowProps {
  board: BoardDto;
  selected: boolean;
  onSelect: (board: BoardDto) => void;
  onDelete: (board: BoardDto) => void;
  onUpdate: (board: BoardDto) => void;
  getBoardLocation: (locationId: number) => string;
}

export function BoardRow({
  board,
  selected,
  onSelect,
  onDelete,
  onUpdate,
  getBoardLocation,
}: BoardRowProps) {
  const displayName = board.label || `Board #${board.id}`;

  return (
    <div
      onClick={() => onSelect(board)}
      className={`group grid min-h-12 cursor-pointer grid-cols-[1.5fr_1.5fr_100px_140px] items-center px-4 transition-colors portrait:max-sm:grid-cols-[1fr_80px] ${
        selected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
      }`}
    >
      {/* Board Label */}
      <div className="flex min-w-0 items-center gap-2">
        {selected && (
          <span className="h-2 w-2 shrink-0 rounded-full bg-blue-500" />
        )}

        <div className="min-w-0">
          <span
            className={`truncate block text-sm font-semibold ${
              selected ? "text-blue-600" : "text-gray-900"
            }`}
          >
            {displayName}
          </span>
          <span className="text-xs text-gray-400">ID: #{board.id}</span>
        </div>
      </div>

      {/* Location */}
      <div className="min-w-0 portrait:max-sm:hidden">
        {board.locationId ? (
          <span className="truncate block text-sm text-gray-600">
            {getBoardLocation(board.locationId)}
          </span>
        ) : (
          <span className="text-sm text-gray-300">—</span>
        )}
      </div>

      {/* Status */}
      <div className="portrait:max-sm:hidden">
        <span
          className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${
            board.isActive
              ? "bg-green-50 text-green-700 border border-green-200"
              : "bg-gray-100 text-gray-500 border border-gray-200"
          }`}
        >
          {board.isActive ? "Aktiv" : "Inaktiv"}
        </span>
      </div>

      {/* Actions */}
      <div className="flex shrink-0 items-center justify-end gap-1">
        <Link
          to={`/boards/${board.id}/qr`}
          onClick={(e) => e.stopPropagation()}
          className="rounded-md px-2 py-1 text-xs font-medium text-gray-600 hover:bg-gray-100 transition"
          title="QR-Code anzeigen"
        >
          QR
        </Link>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onUpdate(board);
          }}
          className="rounded-md px-2 py-1 text-xs font-medium text-blue-600 hover:bg-blue-50 transition"
          title="Board bearbeiten"
        >
          Bearbeiten
        </button>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(board);
          }}
          className="rounded-md px-2 py-1 text-xs font-medium text-red-600 hover:bg-red-50 transition"
          title="Board löschen"
        >
          Löschen
        </button>
      </div>
    </div>
  );
}
