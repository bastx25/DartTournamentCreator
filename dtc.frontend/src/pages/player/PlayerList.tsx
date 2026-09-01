import type { Player } from "../../types/Player";
import { PlayerRow } from "./PlayerRow";

interface PlayerListProps {
  players: Player[];
  selectedPlayer: Player | null;
  onSelectPlayer: (player: Player) => void;
  onDeletePlayer: (player: Player) => void;
}

export function PlayerList({
  players,
  selectedPlayer,
  onSelectPlayer,
  onDeletePlayer,
}: PlayerListProps) {
  return (
    <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 px-4 py-4">
        <h1 className="text-xl font-semibold tracking-tight text-gray-900">
          Players
        </h1>

        <p className="mt-1 text-sm text-gray-500">
          {players.length} Spieler verfügbar
        </p>
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
          />
        ))}
      </div>
    </section>
  );
}
