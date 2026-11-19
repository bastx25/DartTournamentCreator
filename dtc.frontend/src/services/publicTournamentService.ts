import axios from "axios";
import type { PublicTournamentDto } from "../dtos/public/PublicTournamentDto";
import type { TournamentDto } from "../dtos/tournament/TournamentDto";
import type { MatchDto } from "../dtos/match/MatchDto";

const publicClient = axios.create({
  baseURL: "/api/public/tournaments",
  headers: {
    "Content-Type": "application/json",
  },
});

export async function getPublicTournamentByCode(
  code: string,
): Promise<PublicTournamentDto> {
  const normalized = code.trim().toUpperCase();
  const response = await publicClient.get<PublicTournamentDto>(`/${normalized}`);
  return response.data;
}

export async function getPublicTournamentById(
  id: number,
): Promise<PublicTournamentDto> {
  const response = await publicClient.get<PublicTournamentDto>(`/by-id/${id}`);
  return response.data;
}

export async function getActivePublicTournaments(): Promise<TournamentDto[]> {
  const response = await publicClient.get<TournamentDto[]>("/active");
  return response.data;
}

export async function getPublicGroupMatches(
  code: string,
  groupId: number,
): Promise<MatchDto[]> {
  const normalized = code.trim().toUpperCase();
  const response = await publicClient.get<MatchDto[]>(
    `/${normalized}/groups/${groupId}/matches`,
  );
  return response.data ?? [];
}

export async function getPublicBraketMatches(
  code: string,
  braketId: number,
): Promise<MatchDto[]> {
  const normalized = code.trim().toUpperCase();
  const response = await publicClient.get<MatchDto[]>(
    `/${normalized}/brakets/${braketId}/matches`,
  );
  return response.data ?? [];
}
