import { useEffect, useState } from "react";
import { getActivePublicTournaments } from "../services/publicTournamentService";
import type { TournamentDto } from "../dtos/tournament/TournamentDto";

export function useActiveTournaments() {
  const [tournaments, setTournaments] = useState<TournamentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchTournaments() {
      try {
        setLoading(true);
        setError(null);

        const data = await getActivePublicTournaments();
        setTournaments(data);
      } catch (err) {
        console.error("Fehler beim Laden der aktiven Turniere:", err);
        setError("Die aktiven Turniere konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    void fetchTournaments();
  }, []);

  return { tournaments, loading, error };
}
