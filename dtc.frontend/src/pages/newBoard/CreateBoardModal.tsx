import { useState } from "react";
import {
  createBoardDtoSchema,
  type CreateBoardDto,
} from "../../dtos/board/CreateBoardDto";
import type { LocationDto } from "../../dtos/location/LocationDto";

interface CreateBoardModalProps {
  adding: boolean;
  onCancel: () => void;
  onConfirm: (createdBoard: CreateBoardDto) => void;
  locations: LocationDto[];
}

export function CreateBoardModal({
  adding,
  onCancel,
  onConfirm,
  locations,
}: CreateBoardModalProps) {
  const [number, setNumber] = useState(0);
  const [label, setLabel] = useState("");
  const [isActive, setIsActive] = useState<boolean>(false);
  const [locationId, setLocationId] = useState<number>(0);

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateBoardDto, string>>
  >({});

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const formData: CreateBoardDto = {
      locationId,
      number,
      label,
      isActive,
    };

    const result = createBoardDtoSchema.safeParse(formData);

    if (!result.success) {
      const fieldErrors: Partial<Record<keyof CreateBoardDto, string>> = {};

      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof CreateBoardDto;

        if (!fieldErrors[field]) {
          fieldErrors[field] = issue.message;
        }
      });

      setErrors(fieldErrors);
      return;
    }

    setErrors({});

    onConfirm(result.data);
  };

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
      onClick={() => !adding && onCancel()}
      role="presentation"
    >
      <div
        className="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl ring-1 ring-black/5"
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="create-board-title"
      >
        <form onSubmit={handleSubmit}>
          <div className="p-6">
            {/* Icon */}
            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-blue-50">
              <svg
                className="h-6 w-6 text-blue-600"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M16.862 3.487a2.121 2.121 0 013 3L7.5 18.849 3 20l1.151-4.5L16.862 3.487z"
                />
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M15 5l3 3"
                />
              </svg>
            </div>

            <h3
              id="create-board-title"
              className="mt-4 text-lg font-semibold text-gray-900"
            >
              Board erstellen
            </h3>

            <p className="mt-2 text-sm leading-6 text-gray-500">
              Wähle eine Location aus und gib anschließend die Daten des Boards
              ein.
            </p>

            {/* Form fields */}
            <div className="mt-6 space-y-4">
              {/* Location */}
              <div>
                <label
                  htmlFor="location"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Location
                </label>

                <select
                  id="location"
                  value={locationId}
                  onChange={(e) => setLocationId(Number(e.target.value))}
                  disabled={adding}
                  className={`w-full rounded-lg border bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:ring-2 disabled:cursor-not-allowed disabled:bg-gray-100 ${
                    errors.locationId
                      ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                      : "border-gray-300 focus:border-blue-500 focus:ring-blue-500/20"
                  }`}
                >
                  <option value={0}>Location auswählen...</option>

                  {locations.map((location) => (
                    <option key={location.id} value={location.id}>
                      {location.name}, {location.address}
                    </option>
                  ))}
                </select>

                {errors.locationId && (
                  <p className="mt-1.5 text-sm text-red-600">
                    {errors.locationId}
                  </p>
                )}
              </div>

              {/* Number */}
              <div>
                <label
                  htmlFor="number"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Nummer
                </label>

                <input
                  id="number"
                  type="number"
                  value={number}
                  onChange={(e) => setNumber(Number(e.target.value))}
                  disabled={adding}
                  className={`w-full rounded-lg border bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:ring-2 disabled:cursor-not-allowed disabled:bg-gray-100 ${
                    errors.number
                      ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                      : "border-gray-300 focus:border-blue-500 focus:ring-blue-500/20"
                  }`}
                  placeholder="z. B. 1"
                />

                {errors.number && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.number}</p>
                )}
              </div>

              {/* Label */}
              <div>
                <label
                  htmlFor="label"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Label
                </label>

                <input
                  id="label"
                  type="text"
                  value={label}
                  onChange={(e) => setLabel(e.target.value)}
                  disabled={adding}
                  className={`w-full rounded-lg border bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:ring-2 disabled:cursor-not-allowed disabled:bg-gray-100 ${
                    errors.label
                      ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                      : "border-gray-300 focus:border-blue-500 focus:ring-blue-500/20"
                  }`}
                  placeholder="Label"
                />

                {errors.label && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.label}</p>
                )}
              </div>

              {/* Is Active */}
              <div className="flex items-center justify-between rounded-lg border border-gray-300 bg-white px-3 py-2.5">
                <div>
                  <label
                    htmlFor="isActive"
                    className="text-sm font-medium text-gray-700"
                  >
                    Aktiv
                  </label>

                  <p className="text-xs text-gray-500">
                    Soll das Board aktiv sein?
                  </p>
                </div>

                <button
                  type="button"
                  id="isActive"
                  disabled={adding}
                  onClick={() => setIsActive((current) => !current)}
                  className={`relative inline-flex h-6 w-11 shrink-0 cursor-pointer rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 ${
                    isActive ? "bg-blue-600" : "bg-gray-300"
                  }`}
                  role="switch"
                  aria-checked={isActive}
                >
                  <span
                    className={`pointer-events-none inline-block h-5 w-5 translate-y-0.5 rounded-full bg-white shadow-sm transition-transform ${
                      isActive ? "translate-x-5" : "translate-x-0.5"
                    }`}
                  />
                </button>
              </div>

              {errors.isActive && (
                <p className="mt-1.5 text-sm text-red-600">{errors.isActive}</p>
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="flex items-center justify-end gap-3 border-t border-gray-100 bg-gray-50 px-6 py-4">
            <button
              type="button"
              disabled={adding}
              onClick={onCancel}
              className="rounded-lg px-4 py-2.5 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Abbrechen
            </button>

            <button
              type="submit"
              disabled={adding}
              className="inline-flex min-w-32 items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {adding && (
                <svg
                  className="h-4 w-4 animate-spin"
                  viewBox="0 0 24 24"
                  fill="none"
                  aria-hidden="true"
                >
                  <circle
                    className="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    strokeWidth="4"
                  />
                  <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
                  />
                </svg>
              )}

              {adding ? "Speichern..." : "Board erstellen"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
