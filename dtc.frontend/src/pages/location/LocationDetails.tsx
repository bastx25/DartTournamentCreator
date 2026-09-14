import type { LocationDto } from "../../dtos/location/LocationDto";

interface LocationDetailsProps {
  location: LocationDto | null;
}

export function LocationDetails({ location }: LocationDetailsProps) {
  return (
    <aside className="h-fit braketed-xl border border-gray-200 bg-white p-5 shadow-sm lg:sticky lg:top-6">
      {location ? (
        <>
          <div className="mb-5 border-b border-gray-200 pb-5">
            <p className="mb-1 text-xs font-medium uppercase tracking-wider text-gray-400">
              Location Details
            </p>

            <h2 className="text-xl font-semibold text-gray-900">
              {location.name}
            </h2>
          </div>

          <div className="space-y-4">
            <Detail label="Name" value={location.name} />
            <Detail label="Addresse" value={location.address ?? ""} />
          </div>
        </>
      ) : (
        <p className="text-sm text-gray-500">Wähle eine Location aus.</p>
      )}
    </aside>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="block text-xs font-medium uppercase tracking-wide text-gray-400">
        {label}
      </span>

      <strong className="mt-1 block text-sm font-medium text-gray-900">
        {value}
      </strong>
    </div>
  );
}
