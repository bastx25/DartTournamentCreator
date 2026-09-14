import { useEffect, useState } from "react";
import {
  deleteLocation,
  getLocations,
  updateLocation,
  createLocation,
} from "../services/locationService";
import type { LocationDto } from "../dtos/location/LocationDto";
import type { CreateLocationDto } from "../dtos/location/CreateLocationDto";

export function useLocations() {
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [selectedLocation, setSelectedLocation] = useState<LocationDto | null>(
    null,
  );

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [locationToDelete, setLocationToDelete] = useState<LocationDto | null>(
    null,
  );
  const [deleting, setDeleting] = useState(false);

  const [locationToUpdate, setLocationToUpdate] = useState<LocationDto | null>(
    null,
  );
  const [updating, setUpdating] = useState(false);

  const [addLocation, setAddLocation] = useState(false);
  const [adding, setAdding] = useState(false);

  useEffect(() => {
    async function fetchLocations() {
      try {
        setLoading(true);
        setError(null);

        const locations = await getLocations();

        setLocations(locations);

        if (locations.length > 0) {
          setSelectedLocation(locations[0]);
        }
      } catch (error) {
        console.error(error);
        setError("Die Spieler konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    fetchLocations();
  }, []);

  const handleDelete = async () => {
    if (!locationToDelete) return;

    try {
      setDeleting(true);

      await deleteLocation(locationToDelete.id);

      setLocations((currentLocations) =>
        currentLocations.filter(
          (location) => location.id !== locationToDelete.id,
        ),
      );

      if (selectedLocation?.id === locationToDelete.id) {
        setSelectedLocation(null);
      }

      setLocationToDelete(null);
    } catch (error) {
      console.error("Fehler beim Löschen des Spielers:", error);
      setError(
        "Der Spieler konnte nicht gelöscht werden. Ist der Spieler eventuell noch in einem Turnier?",
      );
    } finally {
      setDeleting(false);
    }
  };

  const handleUpdate = async (location: LocationDto) => {
    try {
      setUpdating(true);
      setError(null);

      const updatedLocation = await updateLocation(location.id, {
        name: location.name,
        address: location.address,
      });

      setLocations((currentLocations) =>
        currentLocations.map((currentLocation) =>
          currentLocation.id === updatedLocation.id
            ? updatedLocation
            : currentLocation,
        ),
      );

      setSelectedLocation((currentSelectedLocation) =>
        currentSelectedLocation?.id === updatedLocation.id
          ? updatedLocation
          : currentSelectedLocation,
      );

      setLocationToUpdate(null);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Spielers:", error);
      setError("Der Spieler konnte nicht aktualisiert werden.");
    } finally {
      setUpdating(false);
    }
  };

  const handleAdd = async (location: CreateLocationDto) => {
    try {
      setAdding(true);
      setError(null);

      const newLocation = await createLocation({
        name: location.name,
        address: location.address,
      });

      setLocations((currentLocations) => [...currentLocations, newLocation]);

      setSelectedLocation(newLocation);
    } catch (error) {
      console.error("Fehler beim Aktualisieren des Spielers:", error);
      setError("Der Spieler konnte nicht hinzugefügt werden.");
    } finally {
      setAddLocation(false);
      setAdding(false);
    }
  };

  return {
    locations,
    selectedLocation,
    setSelectedLocation,

    loading,
    error,

    locationToDelete,
    setLocationToDelete,

    deleting,
    handleDelete,

    locationToUpdate,
    setLocationToUpdate,

    updating,
    handleUpdate,

    addLocation,
    setAddLocation,
    adding,
    handleAdd,
  };
}
