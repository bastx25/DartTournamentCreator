import { useEffect, useState } from "react";
import type { Player } from "../../types/player";
import axios from "axios";
import Header from "../../components/Header";

export function PlayerPage() {
  const [players, setPlayers] = useState<Player[]>([]);
  const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [playerToDelete, setPlayerToDelete] = useState<Player | null>(null);
  const [deleting, setDeleting] = useState(false);

  const handleDelete = async () => {
    if (!playerToDelete) return;

    try {
      setDeleting(true);

      await axios.delete(`/api/players/${playerToDelete.id}`);

      setPlayers((currentPlayers) =>
        currentPlayers.filter((player) => player.id !== playerToDelete.id),
      );

      if (selectedPlayer?.id === playerToDelete.id) {
        setSelectedPlayer(null);
      }

      setPlayerToDelete(null);
    } catch (error) {
      console.error("Fehler beim Löschen des Spielers:", error);
    } finally {
      setDeleting(false);
    }
  };

  useEffect(() => {
    const fetchPlayers = async () => {
      try {
        setLoading(true);

        const response = await axios.get<Player[]>("/api/players");

        setPlayers(response.data);

        if (response.data.length > 0) {
          setSelectedPlayer(response.data[0]);
        }
      } catch (err) {
        setError("Die Spieler konnten nicht geladen werden.");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchPlayers();
  }, []);

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 text-white sm:px-6 lg:px-8">
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
            {/* Player List */}
            <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
              {/* Header */}
              <div className="border-b border-gray-200 px-4 py-4">
                <h1 className="text-xl font-semibold tracking-tight text-gray-900">
                  Players
                </h1>

                <p className="mt-1 text-sm text-gray-500">
                  {players.length} Spieler verfügbar
                </p>
              </div>

              {/* Datagrid Header */}
              <div className="grid grid-cols-[1fr_180px_80px] items-center border-b border-gray-200 bg-gray-50 px-4 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500">
                <span>Player</span>
                <span>Nickname</span>
                <span className="text-right">Actions</span>
              </div>

              {/* Rows */}
              <div className="divide-y divide-gray-100">
                {players.map((player) => {
                  const isSelected = selectedPlayer?.id === player.id;

                  return (
                    <div
                      key={player.id}
                      onClick={() => setSelectedPlayer(player)}
                      className={`group grid min-h-[56px] cursor-pointer grid-cols-[1fr_180px_80px] items-center px-4 transition-colors ${
                        isSelected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
                      }`}
                    >
                      {/* Player */}
                      <div className="flex min-w-0 items-center gap-3">
                        {isSelected && (
                          <span className="h-2 w-2 shrink-0 rounded-full bg-blue-500" />
                        )}

                        <span
                          className={`truncate text-sm font-medium ${
                            isSelected ? "text-blue-600" : "text-gray-900"
                          }`}
                        >
                          {player.displayName}
                        </span>
                      </div>

                      {/* Nickname */}
                      <div className="min-w-0">
                        {player.nickname ? (
                          <span className="truncate text-sm text-gray-500">
                            @{player.nickname}
                          </span>
                        ) : (
                          <span className="text-sm text-gray-300">—</span>
                        )}
                      </div>

                      {/* Actions */}
                      <div className="flex items-center justify-end gap-1">
                        {/* Edit */}
                        <button
                          type="button"
                          onClick={(e) => {
                            e.stopPropagation();
                            // TODO: Edit-Funktion
                            console.log("Edit", player);
                          }}
                          className="rounded-md p-1.5 text-gray-400 transition-colors hover:bg-blue-50 hover:text-blue-600"
                          title="Edit player"
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            strokeWidth="2"
                            className="h-4 w-4"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M16.862 4.487a2.25 2.25 0 013.182 3.182L8.25 19.463 4 20.5l1.037-4.25L16.862 4.487z"
                            />
                          </svg>
                        </button>

                        {/* Delete */}
                        <button
                          type="button"
                          onClick={(e) => {
                            e.stopPropagation();
                            setPlayerToDelete(player);
                          }}
                          className="rounded-md p-1.5 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"
                          title="Delete player"
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            strokeWidth="2"
                            className="h-4 w-4"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M6 7h12M9 7V5.5A1.5 1.5 0 0110.5 4h3A1.5 1.5 0 0115 5.5V7m-7 0l.75 12.25A1.75 1.75 0 0010.5 21h3a1.75 1.75 0 001.75-1.75L16 7M10 11v6M14 11v6"
                            />
                          </svg>
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>
            </section>

            {/* Player Details */}
            <aside className="h-fit rounded-xl border border-gray-200 bg-white p-5 shadow-sm lg:sticky lg:top-6">
              {selectedPlayer ? (
                <>
                  <div className="mb-5 border-b border-gray-200 pb-5">
                    <p className="mb-1 text-xs font-medium uppercase tracking-wider text-gray-400">
                      Player Details
                    </p>

                    <h2 className="text-xl font-semibold text-gray-900">
                      {selectedPlayer.displayName}
                    </h2>

                    {selectedPlayer.nickname && (
                      <p className="mt-1 text-sm text-gray-500">
                        @{selectedPlayer.nickname}
                      </p>
                    )}
                  </div>

                  <div className="space-y-4">
                    <div>
                      <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
                        First Name
                      </span>

                      <strong className="mt-1 block text-sm font-medium text-gray-900">
                        {selectedPlayer.firstName}
                      </strong>
                    </div>

                    <div>
                      <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
                        Last Name
                      </span>

                      <strong className="mt-1 block text-sm font-medium text-gray-900">
                        {selectedPlayer.lastName}
                      </strong>
                    </div>

                    {selectedPlayer.nickname && (
                      <div>
                        <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
                          Nickname
                        </span>

                        <strong className="mt-1 block text-sm font-medium text-gray-900">
                          {selectedPlayer.nickname}
                        </strong>
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <p className="text-sm text-gray-500">
                  Wähle einen Spieler aus.
                </p>
              )}
            </aside>
          </div>
        )}
      </main>
      {/* Delete Confirmation Modal */}
      {playerToDelete != null && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4 backdrop-blur-sm"
          onClick={() => !deleting && setPlayerToDelete(null)}
        >
          <div
            className="w-full max-w-md rounded-xl border border-gray-200 bg-white p-6 shadow-2xl"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="mb-5 flex items-start gap-4">
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-red-50 text-red-600">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                  className="h-5 w-5"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M12 9v4m0 4h.01M10.29 3.86l-7.2 12.48A2 2 0 004.82 19h14.36a2 2 0 001.73-2.66l-7.2-12.48a2 2 0 00-3.42 0z"
                  />
                </svg>
              </div>

              <div>
                <h3 className="text-lg font-semibold text-gray-900">
                  Spieler löschen?
                </h3>

                <p className="mt-1 text-sm leading-5 text-gray-500">
                  Möchtest du{" "}
                  <span className="font-medium text-gray-900">
                    {playerToDelete.nickname}
                  </span>{" "}
                  wirklich löschen? Diese Aktion kann nicht rückgängig gemacht
                  werden.
                </p>
              </div>
            </div>

            <div className="flex justify-end gap-3">
              <button
                type="button"
                disabled={deleting}
                onClick={() => setPlayerToDelete(null)}
                className="rounded-lg border border-gray-200 bg-white px-4 py-2 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
              >
                Abbrechen
              </button>

              <button
                type="button"
                disabled={deleting}
                onClick={handleDelete}
                className="rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {deleting ? "Löschen..." : "Löschen"}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
