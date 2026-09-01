import { useEffect, useState } from "react";
import axios from "axios";
import Header from "../../components/Header";

interface Player {
  id: number;
  firstName: string;
  lastName: string;
  nickname: string;
  displayName: string;
}

export function PlayerPage() {
  const [players, setPlayers] = useState<Player[]>([]);
  const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPlayers = async () => {
      try {
        setLoading(true);

        const response = await axios.get<Player[]>("/api/players");

        setPlayers(response.data);

        if (response.data.length > 0) {
          setSelectedPlayer(response.data[0]);
        }
      } catch (err) {
        setError("Die Spieler konnten nicht geladen werden.");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchPlayers();
  }, []);

  return (
    <>
      <Header />

      <div className="player-page">
        {loading && <p>Spieler werden geladen...</p>}

        {error && <p>{error}</p>}

        {!loading && !error && (
          <div className="player-layout">
            <section className="player-list">
              <h1>Players</h1>

              <div className="players-grid">
                {players.map((player) => (
                  <button
                    key={player.id}
                    type="button"
                    className={`player-card ${
                      selectedPlayer?.id === player.id ? "active" : ""
                    }`}
                    onClick={() => setSelectedPlayer(player)}
                  >
                    <span className="player-name">{player.displayName}</span>

                    {player.nickname && (
                      <span className="player-nickname">{player.nickname}</span>
                    )}
                  </button>
                ))}
              </div>
            </section>

            <aside className="player-details">
              {selectedPlayer ? (
                <>
                  <h2>{selectedPlayer.displayName}</h2>

                  <div className="player-info">
                    <div>
                      <span>First Name</span>
                      <strong>{selectedPlayer.firstName}</strong>
                    </div>

                    <div>
                      <span>Last Name</span>
                      <strong>{selectedPlayer.lastName}</strong>
                    </div>

                    {selectedPlayer.nickname && (
                      <div>
                        <span>Nickname</span>
                        <strong>{selectedPlayer.nickname}</strong>
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <p>Wähle einen Spieler aus.</p>
              )}
            </aside>
          </div>
        )}
      </div>
    </>
  );
}
