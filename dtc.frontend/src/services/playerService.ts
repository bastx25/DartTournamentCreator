import axios from "axios";
import type { Player } from "../types/Player";
import type { UpdatePlayer } from "../types/UpdatePlayer";
import type { CreatePlayer } from "../types/CreatePlayer";

export async function getPlayers(): Promise<Player[]> {
  const response = await axios.get<Player[]>("/api/players");

  return response.data;
}

export async function deletePlayer(id: number): Promise<void> {
  await axios.delete(`/api/players/${id}`);
}

export async function updatePlayer(
  id: number,
  data: UpdatePlayer,
): Promise<Player> {
  const response = await axios.put<Player>(`/api/players/${id}`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}

export async function createPlayer(data: CreatePlayer): Promise<Player> {
  const response = await axios.post<Player>(`/api/players`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
