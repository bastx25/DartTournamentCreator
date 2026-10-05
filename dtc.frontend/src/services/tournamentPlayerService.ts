import axios from "axios";
import type { UpdateTournamentPlayerDto } from "../dtos/tournamentPlayer/UpdateTournamentPlayerDto";

export async function updateTPlayer(
  id: number,
  data: UpdateTournamentPlayerDto,
) {
  const response = await axios.put<UpdateTournamentPlayerDto>(
    `/api/players/${id}`,
    data,
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  return response.data;
}
