import axios from "axios";
import type { PlayerDto } from "../dtos/Player/PlayerDto";
import type { UpdatePlayerDto } from "../dtos/Player/UpdatePlayerDto";
import type { CreatePlayerDto } from "../dtos/Player/CreatePlayerDto";

export async function getPlayers(): Promise<PlayerDto[]> {
  const response = await axios.get<PlayerDto[]>("/api/players");

  return response.data;
}

export async function deletePlayer(id: number): Promise<void> {
  await axios.delete(`/api/players/${id}`);
}

export async function updatePlayer(
  id: number,
  data: UpdatePlayerDto,
): Promise<PlayerDto> {
  const response = await axios.put<PlayerDto>(`/api/players/${id}`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}

export async function createPlayer(data: CreatePlayerDto): Promise<PlayerDto> {
  const response = await axios.post<PlayerDto>(`/api/players`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}
