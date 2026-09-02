import { useEffect, useState } from "react";
import { getTournaments } from "../services/tournamentService";
import type { TournamentDto } from "../dtos/Tournament/TournamentDto";

export function useActiveTournaments() {
  const [tournaments, setTournaments] = useState<TournamentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchTournaments() {
      try {
        setLoading(true);
        setError(null);

        const data = await getTournaments({ activeOnly: true, pageSize: 100 });
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
