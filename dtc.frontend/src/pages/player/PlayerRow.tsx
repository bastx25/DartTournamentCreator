import type { PlayerDto } from "../../dtos/Player/PlayerDto";

interface PlayerRowProps {
  player: PlayerDto;
  selected: boolean;
  onSelect: (player: PlayerDto) => void;
  onDelete: (player: PlayerDto) => void;
  onUpdate: (player: PlayerDto) => void;
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
      className={`group grid min-h-11 cursor-pointer grid-cols-[1fr_auto] items-center px-4 transition-colors md:grid-cols-[1fr_1fr_auto] ${
        selected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
      }`}
    >
      {/* Player */}
      <div className="flex min-w-0 items-center gap-2">
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

      {/* Nickname - nur Desktop */}
      <div className="hidden min-w-0 px-4 md:block">
        {player.nickname ? (
          <span className="truncate text-sm text-gray-500">
            @{player.nickname}
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
            onUpdate(player);
          }}
          className="rounded-md px-1.5 py-1 text-blue-400 transition-colors hover:bg-blue-50 hover:text-blue-600"
          title="Edit player"
        >
          Edit
        </button>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(player);
          }}
          className="rounded-md px-1.5 py-1 text-red-400 transition-colors hover:bg-red-50 hover:text-red-600"
          title="Delete player"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
