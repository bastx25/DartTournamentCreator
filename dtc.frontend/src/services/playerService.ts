import axios from "axios";
import type { Dto } from "../dtos/player/PlayerDto";
import type { UpdatePlayerDto } from "../dtos/player/UpdatePlayerDto";
import type { CreatePlayerDto } from "../dtos/player/CreatePlayerDto";

export async function getPlayers(): Promise<Dto[]> {
  const response = await axios.get<Dto[]>("/api/players");

  return response.data;
}

export async function deletePlayer(id: number): Promise<void> {
  await axios.delete(`/api/players/${id}`);
}

export async function updatePlayer(
  id: number,
  data: UpdatePlayerDto,
): Promise<Dto> {
  const response = await axios.put<Dto>(`/api/players/${id}`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}

export async function createPlayer(data: CreatePlayerDto): Promise<Dto> {
  const response = await axios.post<Dto>(`/api/players`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
