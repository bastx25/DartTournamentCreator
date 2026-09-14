import type { LocationDto } from "../../dtos/location/LocationDto";
import { LocationRow } from "./LocationRow";

interface LocationListProps {
  locations: LocationDto[];
  selectedLocation: LocationDto | null;
  onSelectLocation: (location: LocationDto) => void;
  onDeleteLocation: (location: LocationDto) => void;
  onUpdateLocation: (location: LocationDto) => void;
  onAddLocation: (addLocation: boolean) => void;
}

export function LocationList({
  locations,
  selectedLocation,
  onSelectLocation,
  onDeleteLocation,
  onUpdateLocation,
  onAddLocation,
}: LocationListProps) {
  return (
    <section className="overflow-hidden braketed-xl border border-gray-200 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-gray-200 px-4 py-4">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-gray-900">
            Locations
          </h1>

          <p className="mt-1 text-sm text-gray-500">
            {locations.length} Locations verfügbar
          </p>
        </div>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onAddLocation(true);
          }}
          className="flex h-9 w-9 items-center justify-center braketed-lg bg-green-600 text-xl font-semibold text-white shadow-sm transition hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
          title="Locations hinzufügen"
        >
          +
        </button>
      </div>

      <div className="grid grid-cols-[1fr_580px_80px] items-center border-b border-gray-200 bg-gray-50 px-4 py-2.5 text-xs font-medium uppercase tracking-wide text-gray-500">
        <span>Location</span>
        <span>Address</span>
        <span className="text-right">Actions</span>
      </div>

      <div className="divide-y divide-gray-100">
        {locations.map((location) => (
          <LocationRow
            key={location.id}
            location={location}
            selected={selectedLocation?.id === location.id}
            onSelect={onSelectLocation}
            onDelete={onDeleteLocation}
            onUpdate={onUpdateLocation}
          />
        ))}
      </div>
    </section>
  );
}
