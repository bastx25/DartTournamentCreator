import type { Dto } from "../../dtos/player/PlayerDto";
import { PlayerRow } from "./PlayerRow";

interface PlayerListProps {
  players: Dto[];
  selectedPlayer: Dto | null;
  onSelectPlayer: (player: Dto) => void;
  onDeletePlayer: (player: Dto) => void;
  onUpdatePlayer: (player: Dto) => void;
  onAddPlayer: (addPlayer: boolean) => void;
}

export function PlayerList({
  players,
  selectedPlayer,
  onSelectPlayer,
  onDeletePlayer,
  onUpdatePlayer,
  onAddPlayer,
}: PlayerListProps) {
  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-gray-200 px-4 py-4">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-gray-900">
            Players
          </h1>

          <p className="mt-1 text-sm text-gray-500">
            {players.length} Spieler verfügbar
          </p>
        </div>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onAddPlayer(true);
          }}
          className="flex h-9 w-9 items-center justify-center rounded-lg bg-green-600 text-xl font-semibold text-white shadow-sm transition hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
          title="Spieler hinzufügen"
        >
          +
        </button>
      </div>

      <div className="grid grid-cols-[1fr_180px_80px] items-center border-b border-gray-200 bg-gray-50 px-4 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500">
        <span>Player</span>
        <span>Nickname</span>
        <span className="text-right">Actions</span>
      </div>

      <div className="divide-y divide-gray-100">
        {players.map((player) => (
          <PlayerRow
            key={player.id}
            player={player}
            selected={selectedPlayer?.id === player.id}
            onSelect={onSelectPlayer}
            onDelete={onDeletePlayer}
            onUpdate={onUpdatePlayer}
          />
        ))}
      </div>
    </section>
  );
}
