import axios from "axios";
import type { UpdateTournamentPlayerDto } from "../dtos/tournamentPlayer/UpdateTournamentPlayerDto";
import type { TournamentPlayerDto } from "../dtos/tournamentPlayer/TournamentPlayerDto";

export async function updateTPlayer(
  id: number,
  data: UpdateTournamentPlayerDto,
) {
  const response = await axios.put<UpdateTournamentPlayerDto>(
    `/api/tournamentplayers/${id}`,
    data,
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  return response.data;
}

// export async function updateTPlayers(tplayers: TournamentPlayerDto[]) {
//   await Promise.all(
//     tplayers.map((tplayer) => {
//       const data: UpdateTournamentPlayerDto = {
//         tournamentId: tplayer.tournamentId,
//         playerId: tplayer.playerId,
//         isQualified: tplayer.isQualified,
//       };

//       return updateTPlayer(tplayer.id, data);
//     }),
//   );
// }

export async function updateTPlayers(tplayers: TournamentPlayerDto[]) {
  const response = await axios.put<TournamentPlayerDto>(
    `/api/tournamentplayers`,
    tplayers,
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  return response.data;
}
