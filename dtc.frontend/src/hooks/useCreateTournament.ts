import { useState } from "react";
import type { CreateTournamentDto } from "../dtos/Tournament/CreateTournamentDto";
import { createTournament } from "../services/tournamentService";

export function useCreateTournament() {
  const [adding, setAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAdd = async (tournament: CreateTournamentDto) => {
    try {
      setAdding(true);
      setError(null);

      return await createTournament(tournament);
    } catch (error) {
      console.error("Fehler beim Erstellen des Turniers:", error);
      setError("Das Turnier konnte nicht erstellt werden.");
      return null;
    } finally {
      setAdding(false);
    }
  };

  return {
    adding,
    error,
    handleAdd,
  };
}
