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

export interface TournamentQuery {
  activeOnly?: boolean;
  status?: number;
  sortBy?: "startDate" | "name";
  isDescending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

export async function getTournaments(
  query: TournamentQuery = {},
): Promise<TournamentDto[]> {
  const response = await axios.get<TournamentDto[]>("/api/tournaments", {
    params: query,
  });
  return response.data;
}

export async function generateKnockout(id: number): Promise<void> {
  await axios.post(`/api/tournaments/${id}/generate-knockout`, null, {
    headers: {
      "Content-Type": "application/json",
    },
  });
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
