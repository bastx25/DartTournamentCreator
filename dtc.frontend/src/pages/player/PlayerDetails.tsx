import type { PlayerDto } from "../../dtos/Player/PlayerDto";

interface PlayerDetailsProps {
  player: PlayerDto | null;
}

export function PlayerDetails({ player }: PlayerDetailsProps) {
  return (
    <aside className="h-fit rounded-xl border border-gray-200 bg-white p-5 shadow-sm lg:sticky lg:top-6">
      {player ? (
        <>
          <div className="mb-5 border-b border-gray-200 pb-5">
            <p className="mb-1 text-xs font-medium uppercase tracking-wider text-gray-400">
              Player Details
            </p>

            <h2 className="text-xl font-semibold text-gray-900">
              {player.displayName}
            </h2>

            {player.nickname && (
              <p className="mt-1 text-sm text-gray-500">@{player.nickname}</p>
            )}
          </div>

          <div className="space-y-4">
            <Detail label="First Name" value={player.firstName} />
            <Detail label="Last Name" value={player.lastName} />

            {player.nickname && (
              <Detail label="Nickname" value={player.nickname} />
            )}
          </div>
        </>
      ) : (
        <p className="text-sm text-gray-500">Wähle einen Spieler aus.</p>
      )}
    </aside>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
        {label}
      </span>

      <strong className="mt-1 block text-sm font-medium text-gray-900">
        {value}
      </strong>
    </div>
  );
}
