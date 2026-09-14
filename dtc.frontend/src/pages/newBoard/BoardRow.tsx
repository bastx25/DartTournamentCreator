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
  return (
    <div
      onClick={() => onSelect(board)}
      className={`group grid min-h-11 cursor-pointer grid-cols-[1fr_180px_180px_80px] items-center px-4 transition-colors portrait:max-sm:grid-cols-[1fr_80px] ${
        selected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
      }`}
    >
      {/* Board */}
      <div className="flex min-w-0 items-center gap-2 portrait:max-sm:hidden">
        {selected && (
          <span className="h-2 w-2 shrink-0 rounded-full bg-blue-500" />
        )}

        <span
          className={`truncate text-sm font-medium ${
            selected ? "text-blue-600" : "text-gray-900"
          }`}
        >
          {board.number}
        </span>
      </div>

      {/* Label */}
      <div className="min-w-0">
        {board.label ? (
          <span className="truncate text-sm text-gray-500">{board.label}</span>
        ) : (
          <span className="text-sm text-gray-300">—</span>
        )}
      </div>

      {/* Location */}
      <div className="min-w-0 shrink-0 portrait:max-sm:hidden">
        {board.locationId ? (
          <span className="truncate text-sm text-gray-500">
            {getBoardLocation(board.locationId)}
          </span>
        ) : (
          <span className="text-sm text-gray-300">—</span>
        )}
      </div>

      {/* Actions */}
      <div className="flex shrink-0 items-center justify-end gap-1">
        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onUpdate(board);
          }}
          className="rounded-md px-1.5 py-1 text-blue-400 transition-colors hover:bg-blue-50 hover:text-blue-600"
          title="Edit board"
        >
          Edit
        </button>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(board);
          }}
          className="rounded-md px-1.5 py-1 text-red-400 transition-colors hover:bg-red-50 hover:text-red-600"
          title="Delete board"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
