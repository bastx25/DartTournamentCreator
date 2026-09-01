import { useEffect, useState } from "react";
import { deletePlayer, getPlayers } from "../services/playerService";
import type { Player } from "../types/Player";

export function usePlayers() {
  const [players, setPlayers] = useState<Player[]>([]);
  const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [playerToDelete, setPlayerToDelete] = useState<Player | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    async function fetchPlayers() {
      try {
        setLoading(true);

        const players = await getPlayers();

        setPlayers(players);

        if (players.length > 0) {
          setSelectedPlayer(players[0]);
        }
      } catch (error) {
        console.error(error);
        setError("Die Spieler konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    fetchPlayers();
  }, []);

  const handleDelete = async () => {
    if (!playerToDelete) return;

    try {
      setDeleting(true);

      await deletePlayer(playerToDelete.id);

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

  return {
    players,
    selectedPlayer,
    setSelectedPlayer,

    loading,
    error,

    playerToDelete,
    setPlayerToDelete,

    deleting,
    handleDelete,
  };
}
