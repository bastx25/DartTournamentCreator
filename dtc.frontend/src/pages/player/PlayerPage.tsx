import Header from "../../components/Header";

import { PlayerList } from "./PlayerList";

import { usePlayers } from "../../hooks/usePlayers";
import { PlayerDetails } from "./PlayerDetails";
import { DeletePlayerModal } from "./DeletePlayerModal";
import { UpdatePlayerModal } from "./UpdatePlayerModal";
import { CreatePlayerModal } from "./CreatePlayerModal";

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
    playerToUpdate,
    setPlayerToUpdate,
    updating,
    handleUpdate,

    addPlayer,
    setAddPlayer,
    adding,
    handleAdd,
  } = usePlayers();

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 sm:px-6 lg:px-8">
        {loading && (
          <div className="flex min-h-75 items-center justify-center">
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
              onUpdatePlayer={setPlayerToUpdate}
              onAddPlayer={setAddPlayer}
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

      {playerToUpdate && (
        <UpdatePlayerModal
          player={playerToUpdate}
          updating={updating}
          onCancel={() => setPlayerToUpdate(null)}
          onConfirm={handleUpdate}
        />
      )}

      {addPlayer && (
        <CreatePlayerModal
          adding={adding}
          onCancel={() => {
            setAddPlayer(false);
          }}
          onConfirm={handleAdd}
        />
      )}
    </>
  );
}
