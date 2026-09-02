import { useState } from "react";
import { createLocation } from "../services/locationService";
import type { CreateLocationDto } from "../dtos/location/CreateLocationDto";

export function useCreateLocation() {
  const [adding, setAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAdd = async (location: CreateLocationDto) => {
    try {
      setAdding(true);
      setError(null);

      return await createLocation(location);
    } catch (error) {
      console.error("Fehler beim Erstellen der Location:", error);
      setError("Die Location konnte nicht erstellt werden.");
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
