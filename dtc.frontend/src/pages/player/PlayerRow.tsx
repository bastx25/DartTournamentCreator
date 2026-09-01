import type { Player } from "../../types/Player";

interface PlayerRowProps {
  player: Player;
  selected: boolean;
  onSelect: (player: Player) => void;
  onDelete: (player: Player) => void;
  onUpdate: (player: Player) => void;
}

export function PlayerRow({
  player,
  selected,
  onSelect,
  onDelete,
  onUpdate,
}: PlayerRowProps) {
  return (
    <div
      onClick={() => onSelect(player)}
      className={`group grid min-h-14 cursor-pointer grid-cols-[1fr_180px_80px] items-center px-4 transition-colors ${
        selected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
      }`}
    >
      <div className="flex min-w-0 items-center gap-3">
        {selected && (
          <span className="h-2 w-2 shrink-0 rounded-full bg-blue-500" />
        )}

        <span
          className={`truncate text-sm font-medium ${
            selected ? "text-blue-600" : "text-gray-900"
          }`}
        >
          {player.displayName}
        </span>
      </div>

      <div className="min-w-0">
        {player.nickname ? (
          <span className="truncate text-sm text-gray-500">
            @{player.nickname}
          </span>
        ) : (
          <span className="text-sm text-gray-300">—</span>
        )}
      </div>

      <div className="flex items-center justify-end gap-1">
        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onUpdate(player);
          }}
          className="rounded-md p-1.5 text-blue-400 transition-colors hover:bg-blue-50 hover:text-blue-600"
          title="Edit player"
        >
          {/* Edit Icon */}
          Edit
        </button>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(player);
          }}
          className="rounded-md p-1.5 text-red-400 transition-colors hover:bg-red-50 hover:text-red-600"
          title="Delete player"
        >
          {/* Delete Icon */}
          Delete
        </button>
      </div>
    </div>
  );
}
