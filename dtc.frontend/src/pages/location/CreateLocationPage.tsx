import { useState } from "react";
import { useNavigate } from "react-router";
import Header from "../../components/Header";
import {
  createLocationDtoSchema,
  type CreateLocationDto,
} from "../../dtos/location/CreateLocationDto";
import { useCreateLocation } from "../../hooks/useCreateLocation";

export function CreateLocationPage() {
  const navigate = useNavigate();
  const { adding, error, handleAdd } = useCreateLocation();

  const [formData, setFormData] = useState<CreateLocationDto>({
    name: "",
    address: null,
  });

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateLocationDto, string>>
  >({});

  const updateField = <K extends keyof CreateLocationDto>(
    field: K,
    value: CreateLocationDto[K],
  ) => {
    setFormData((current) => ({ ...current, [field]: value }));
    setErrors((current) => ({ ...current, [field]: undefined }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = createLocationDtoSchema.safeParse(formData);

    if (!result.success) {
      const fieldErrors: Partial<Record<keyof CreateLocationDto, string>> = {};

      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof CreateLocationDto;

        if (!fieldErrors[field]) {
          fieldErrors[field] = issue.message;
        }
      });

      setErrors(fieldErrors);
      return;
    }

    setErrors({});

    const location = await handleAdd(result.data);

    if (location) {
      navigate("/locations");
    }
  };

  const inputClass = (field: keyof CreateLocationDto) =>
    `
      mt-2 block w-full rounded-lg border bg-white px-3 py-2.5
      text-sm text-gray-900 shadow-sm outline-none transition
      placeholder:text-gray-400
      focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20
      disabled:cursor-not-allowed disabled:bg-gray-100
      ${
        errors[field]
          ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
          : "border-gray-300"
      }
    `;

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <main className="mx-auto w-full max-w-2xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold tracking-tight text-gray-900">
            Location erstellen
          </h1>
          <p className="mt-2 text-sm text-gray-600">
            Erstelle eine neue Location und hinterlege die grundlegenden
            Informationen.
          </p>
        </div>

        {error && (
          <div className="mb-6 rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        <div className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
          <form onSubmit={handleSubmit}>
            <div className="space-y-6 p-6 sm:p-8">
              {/* Name */}
              <div>
                <div className="flex items-center justify-between">
                  <label
                    htmlFor="name"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Name
                  </label>
                  <span className="text-xs text-gray-400">
                    max. 100 Zeichen
                  </span>
                </div>

                <input
                  id="name"
                  type="text"
                  maxLength={100}
                  value={formData.name}
                  onChange={(event) => updateField("name", event.target.value)}
                  disabled={adding}
                  className={inputClass("name")}
                  placeholder="z. B. DartForge Linz"
                />

                {errors.name && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.name}</p>
                )}
              </div>

              {/* Address */}
              <div>
                <div className="flex items-center justify-between">
                  <label
                    htmlFor="address"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Adresse
                  </label>
                  <span className="text-xs text-gray-400">
                    max. 200 Zeichen
                  </span>
                </div>

                <textarea
                  id="address"
                  rows={3}
                  maxLength={200}
                  value={formData.address ?? ""}
                  onChange={(event) =>
                    updateField("address", event.target.value || null)
                  }
                  disabled={adding}
                  className={inputClass("address")}
                  placeholder="z. B. Musterstraße 1, 4020 Linz"
                />

                {errors.address && (
                  <p className="mt-1.5 text-sm text-red-600">
                    {errors.address}
                  </p>
                )}
              </div>
            </div>

            <div className="flex flex-col-reverse gap-3 border-t border-gray-200 bg-gray-50 px-6 py-4 sm:flex-row sm:justify-end sm:px-8">
              <button
                type="button"
                disabled={adding}
                onClick={() => navigate(-1)}
                className="rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 shadow-sm transition hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:opacity-60"
              >
                Abbrechen
              </button>

              <button
                type="submit"
                disabled={adding}
                className="inline-flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/40 disabled:cursor-not-allowed disabled:opacity-60"
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
                {adding ? "Speichern..." : "Location erstellen"}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
