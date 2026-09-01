import { DeletePlayerModal } from "../../components/DeletePlayerModal";
import Header from "../../components/Header";
import { PlayerDetails } from "../../components/PlayerDetails";
import { PlayerList } from "../../components/PlayerList";

import { usePlayers } from "../../hooks/usePlayers";

export function PlayerPage() {
  const {
    players,
    selectedPlayer,
    setSelectedPlayer,
    loading,
    error,
    playerToDelete,
    setPlayerToDelete,
    deleting,
    handleDelete,
  } = usePlayers();

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 sm:px-6 lg:px-8">
        {loading && (
          <div className="flex min-h-[300px] items-center justify-center">
            <p className="text-sm text-gray-400">Spieler werden geladen...</p>
          </div>
        )}

        {error && (
          <div className="mx-auto max-w-7xl rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-400">{error}</p>
          </div>
        )}

        {!loading && !error && (
          <div className="mx-auto grid max-w-7xl grid-cols-1 gap-6 lg:grid-cols-[1fr_350px]">
            <PlayerList
              players={players}
              selectedPlayer={selectedPlayer}
              onSelectPlayer={setSelectedPlayer}
              onDeletePlayer={setPlayerToDelete}
            />

            <PlayerDetails player={selectedPlayer} />
          </div>
        )}
      </main>

      {playerToDelete && (
        <DeletePlayerModal
          player={playerToDelete}
          deleting={deleting}
          onCancel={() => setPlayerToDelete(null)}
          onConfirm={handleDelete}
        />
      )}
    </>
  );
}
