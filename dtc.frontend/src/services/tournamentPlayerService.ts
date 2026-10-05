import axios from "axios";
import type { UpdateTournamentPlayerDto } from "../dtos/tournamentPlayer/UpdateTournamentPlayerDto";
import type { TournamentPlayerDto } from "../dtos/tournamentPlayer/TournamentPlayerDto";

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

export async function updateTPlayers(tplayers: TournamentPlayerDto[]) {
  tplayers.map((tplayer: TournamentPlayerDto) => {
    console.log(tplayer.id);
  });
}
