import axios from "axios";
import type { CreateTournamentDto } from "../dtos/Tournament/CreateTournamentDto";
import type { TournamentDto } from "../dtos/Tournament/TournamentDto";
import type { GenerateGroupsDto } from "../dtos/MatchMaker/GenerateGroupsDto";

export async function createTournament(
  data: CreateTournamentDto,
): Promise<TournamentDto> {
  const response = await axios.post<TournamentDto>("/api/tournaments", data, {
    headers: {
      "Content-Type": "application/json",
    },
  });

  return response.data;
}

export async function getTournaments(): Promise<TournamentDto[]> {
  const response = await axios.get<TournamentDto[]>("/api/tournaments");
  return response.data;
}

export async function getTournament(id: number): Promise<TournamentDto> {
  const response = await axios.get<TournamentDto>(`/api/tournaments/${id}`);
  return response.data;
}

export async function generateGroups(
  id: number,
  data: GenerateGroupsDto,
): Promise<void> {
  await axios.post(`/api/tournaments/${id}/generate-groups`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
}
