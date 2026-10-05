import { useParams } from "react-router";
import Header from "../../components/Header";
import { useEffect, useState } from "react";
import { getTournament, getTPlayers } from "../../services/tournamentService";
import type { TournamentPlayerDto } from "../../dtos/tournamentPlayer/TournamentPlayerDto";
import { getPlayers } from "../../services/playerService";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import type { PlayerDto } from "../../dtos/player/PlayerDto";
import { updateTPlayers } from "../../services/tournamentPlayerService";

export function ManageTournamentPlayers() {
  const { id } = useParams();
  const tournamentId = Number(id);
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [tplayers, setTPlayers] = useState<TournamentPlayerDto[]>([]);
  const [selectedPlayerIds, setSelectedPlayerIds] = useState<number[]>([]);
  const [tournament, setTournament] = useState<TournamentDto | null>(null);

  const selectedPlayers = selectedPlayerIds.length;

  const isValidTournamentId =
    Number.isInteger(tournamentId) && tournamentId > 0;

  const togglePlayer = (playerId: number) => {
    setSelectedPlayerIds((current) =>
      current.includes(playerId)
        ? current.filter((id) => id !== playerId)
        : [...current, playerId],
    );
  };

  useEffect(() => {
    if (!isValidTournamentId) {
      return;
    }

    async function load() {
      try {
        const [tournamentData, playerData, tPlayerData] = await Promise.all([
          getTournament(tournamentId),
          getPlayers(),
          getTPlayers(tournamentId),
        ]);

        setTournament(tournamentData);
        setPlayers(playerData);
        setTPlayers(tPlayerData);

        setSelectedPlayerIds(
          tPlayerData
            .filter((p: TournamentPlayerDto) => p.isQualified)
            .map((p: TournamentPlayerDto) => p.playerId),
        );
      } catch (err) {
        console.error(err);
      }
    }

    void load();
  }, [tournamentId, isValidTournamentId]);

  const handleSaveQualified = async () => {
    try {
      tplayers.forEach((p: TournamentPlayerDto) => {
        if (selectedPlayerIds.includes(p.id)) {
          p.isQualified = true;
        }
      });

      await updateTPlayers(tplayers);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <>
      <Header />

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_360px]">
        <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
          <div className="flex justify-end border-t border-gray-200 px-6 py-4 sm:px-8">
            <button
              type="button"
              onClick={() => {
                console.log("Ausgewählte Spieler:", selectedPlayerIds);
                handleSaveQualified();
              }}
              className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            >
              Speichern
            </button>
          </div>

          <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
            <h2 className="text-lg font-semibold text-gray-900">Spieler</h2>
            <p className="mt-1 text-sm text-gray-500">
              Lege fest, welche Spieler an der Gruppenphase teilnehmen.
            </p>
          </div>

          <div className="flex items-center justify-between border-b border-gray-100 bg-gray-50 px-6 py-3 sm:px-8">
            <span className="text-sm font-medium text-gray-700">
              {selectedPlayers} von {players.length} ausgewählt
            </span>
            <div className="flex gap-3">
              <button
                type="button"
                onClick={() => setSelectedPlayerIds(players.map((p) => p.id))}
                className="text-xs font-medium text-blue-600 hover:text-blue-700"
              >
                Alle auswählen
              </button>
              <button
                type="button"
                onClick={() => setSelectedPlayerIds([])}
                className="text-xs font-medium text-gray-500 hover:text-gray-700"
              >
                Keine
              </button>
            </div>
          </div>

          <div className="divide-y divide-gray-100">
            {players.map((player) => {
              const selected = selectedPlayerIds.includes(player.id);

              return (
                <label
                  key={player.id}
                  className="flex cursor-pointer items-center gap-3 px-6 py-3 transition hover:bg-gray-50 sm:px-8"
                >
                  <input
                    type="checkbox"
                    checked={selected}
                    onChange={() => togglePlayer(player.id)}
                    className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />

                  <span className="min-w-0">
                    <span className="block text-sm font-medium text-gray-900">
                      {player.displayName}
                    </span>

                    {player.nickname && (
                      <span className="block text-xs text-gray-500">
                        {player.nickname}
                      </span>
                    )}
                  </span>
                </label>
              );
            })}

            {players.length === 0 && (
              <div className="px-8 py-10 text-center text-sm text-gray-500">
                Es sind keine Spieler vorhanden.
              </div>
            )}
          </div>
        </section>
      </div>
    </>
  );
}
