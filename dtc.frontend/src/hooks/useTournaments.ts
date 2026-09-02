import { useEffect, useState } from "react";
import { getTournaments } from "../services/tournamentService";
import type { TournamentDto } from "../dtos/Tournament/TournamentDto";

export function useTournaments() {
  const [tournaments, setTournaments] = useState<TournamentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchTournaments() {
      try {
        setLoading(true);
        setError(null);

        const tournaments = await getTournaments();
        setTournaments(tournaments);
      } catch (error) {
        console.error("Fehler beim Laden der Turniere:", error);
        setError("Die Turniere konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    fetchTournaments();
  }, []);

  return {
    tournaments,
    loading,
    error,
  };
}
