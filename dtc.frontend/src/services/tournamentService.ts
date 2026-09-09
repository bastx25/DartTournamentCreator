import axios from "axios";
import type { CreateTournamentDto } from "../dtos/tournament/CreateTournamentDto";
import type { TournamentDto } from "../dtos/tournament/TournamentDto";
import type { GenerateGroupsDto } from "../dtos/matchMaker/GenerateGroupDto";
import type { GroupDto } from "../dtos/group/GroupDto";
import type { RoundDto } from "../dtos/rounds/RoundDto";

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

export async function generateKnockout(
  id: number,
  data: GenerateGroupsDto,
): Promise<void> {
  await axios.post(`/api/tournaments/${id}/generate-knockout`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
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

export async function getTournament(id: number): Promise<TournamentDto> {
  const response = await axios.get<TournamentDto>(`/api/tournaments/${id}`);
  return response.data;
}

export async function deleteTournament(id: number): Promise<void> {
  await axios.delete(`/api/tournaments/${id}`);
}

export async function getGroupsByTournamentId(
  tournamentId: number,
): Promise<GroupDto[]> {
  const response = await axios.get<GroupDto[]>(
    `/api/tournaments/${tournamentId}/groups`,
  );
  return response.data ?? [];
}

export async function getRoundsByTournamentId(
  tournamentId: number,
): Promise<RoundDto[]> {
  const response = await axios.get<RoundDto[]>(
    `/api/tournaments/${tournamentId}/rounds`,
  );

  return response.data ?? [];
}
