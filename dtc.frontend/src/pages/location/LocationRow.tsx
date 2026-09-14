import type { LocationDto } from "../../dtos/location/LocationDto";

interface LocationRowProps {
  location: LocationDto;
  selected: boolean;
  onSelect: (location: LocationDto) => void;
  onDelete: (location: LocationDto) => void;
  onUpdate: (location: LocationDto) => void;
}

export function LocationRow({
  location,
  selected,
  onSelect,
  onDelete,
  onUpdate,
}: LocationRowProps) {
  return (
    <div
      onClick={() => onSelect(location)}
      className={`group grid min-h-11 cursor-pointer grid-cols-[1fr_580px_80px] items-center px-4 transition-colors ${
        selected ? "bg-blue-50" : "bg-white hover:bg-gray-50"
      }`}
    >
      {/* Location */}
      <div className="flex min-w-0 items-center gap-2">
        {selected && (
          <span className="h-2 w-2 shrink-0 rounded-full bg-blue-500" />
        )}

        <span
          className={`truncate text-sm font-medium ${
            selected ? "text-blue-600" : "text-gray-900"
          }`}
        >
          {location.name}
        </span>
      </div>

      {/* Address */}
      <div className="min-w-0 ">
        {location.address ? (
          <span className="truncate text-sm text-gray-500">
            {location.address}
          </span>
        ) : (
          <span className="text-sm text-gray-300">—</span>
        )}
      </div>

      {/* Actions */}
      <div className="flex shrink-0 items-center justify-end gap-1">
        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onUpdate(location);
          }}
          className="rounded-md px-1.5 py-1 text-blue-400 transition-colors hover:bg-blue-50 hover:text-blue-600"
          title="Edit location"
        >
          Edit
        </button>

        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(location);
          }}
          className="rounded-md px-1.5 py-1 text-red-400 transition-colors hover:bg-red-50 hover:text-red-600"
          title="Delete location"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
