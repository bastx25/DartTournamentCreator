import axios from "axios";
import type { Player } from "../types/Player";

export async function getPlayers(): Promise<Player[]> {
  const response = await axios.get<Player[]>("/api/players");

  return response.data;
}

export async function deletePlayer(id: number): Promise<void> {
  await axios.delete(`/api/players/${id}`);
}
