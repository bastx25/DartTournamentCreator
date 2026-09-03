import { useEffect, useState } from "react";
import {
  deletePlayer,
  getPlayers,
  updatePlayer,
  createPlayer,
} from "../services/playerService";
import type { PlayerDto } from "../dtos/player/PlayerDto";
import type { CreatePlayerDto } from "../dtos/Player/CreatePlayerDto";

export function usePlayers() {
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [selectedPlayer, setSelectedPlayer] = useState<PlayerDto | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [playerToDelete, setPlayerToDelete] = useState<PlayerDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const [playerToUpdate, setPlayerToUpdate] = useState<PlayerDto | null>(null);
  const [updating, setUpdating] = useState(false);

  const [addPlayer, setAddPlayer] = useState(false);
  const [adding, setAdding] = useState(false);

  useEffect(() => {
    async function fetchPlayers() {
      try {
        setLoading(true);
        setError(null);

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
      setError("Der Spieler konnte nicht gelöscht werden.");
    } finally {
      setDeleting(false);
    }
  };

  const handleUpdate = async (player: PlayerDto) => {
    try {
      setUpdating(true);
      setError(null);

      const updatedPlayer = await updatePlayer(player.id, {
        firstName: player.firstName,
        lastName: player.lastName,
        nickname: player.nickname,
      });

      setPlayers((currentPlayers) =>
        currentPlayers.map((currentPlayer) =>
          currentPlayer.id === updatedPlayer.id ? updatedPlayer : currentPlayer,
        ),
      );

      setSelectedPlayer((currentSelectedPlayer) =>
        currentSelectedPlayer?.id === updatedPlayer.id
          ? updatedPlayer
          : currentSelectedPlayer,
      );

      setPlayerToUpdate(null);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Spielers:", error);
      setError("Der Spieler konnte nicht aktualisiert werden.");
    } finally {
      setUpdating(false);
    }
  };

  const handleAdd = async (player: CreatePlayerDto) => {
    try {
      setAdding(true);
      setError(null);

      const newPlayer = await createPlayer({
        firstName: player.firstName,
        lastName: player.lastName,
        nickname: player.nickname,
      });

      setPlayers((currentPlayers) => [...currentPlayers, newPlayer]);

      setSelectedPlayer(newPlayer);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Spielers:", error);
      setError("Der Spieler konnte nicht aktualisiert werden.");
    } finally {
      setAddPlayer(false);
      setAdding(false);
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

    playerToUpdate,
    setPlayerToUpdate,

    updating,
    handleUpdate,

    addPlayer,
    setAddPlayer,
    adding,
    handleAdd,
  };
}
