import { useState } from "react";
import { useNavigate } from "react-router";
import Header from "../../components/Header";
import {
  createTournamentDtoSchema,
  type CreateTournamentDto,
} from "../../dtos/Tournament/CreateTournamentDto";
import { TournamentMode } from "../../enums/TournamentMode";
import { TournamentStatus } from "../../enums/TournamentStatus";
import { useCreateTournament } from "../../hooks/useCreateTournament";
const modeOptions = [
  {
    value: TournamentMode.GroupStage,
    label: "Gruppenphase",
    description: "Das Turnier wird in Gruppen ausgespielt.",
  },
  {
    value: TournamentMode.GrouStageandKnockout,
    label: "Gruppenphase & K.-o.-Phase",
    description: "Auf die Gruppenphase folgt eine K.-o.-Runde.",
  },
];

const statusOptions = [
  { value: TournamentStatus.Draft, label: "Entwurf" },
  { value: TournamentStatus.Scheduled, label: "Geplant" },
  { value: TournamentStatus.InProgress, label: "Laufend" },
  { value: TournamentStatus.Completed, label: "Abgeschlossen" },
  { value: TournamentStatus.Cancelled, label: "Abgebrochen" },
];

export function CreateTournamentPage() {
  const navigate = useNavigate();
  const { adding, error, handleAdd } = useCreateTournament();

  const [startDateInput, setStartDateInput] = useState("");

  const [formData, setFormData] = useState<CreateTournamentDto>({
    name: "",
    startDate: "",
    description: null,
    mode: TournamentMode.GroupStage,
    status: TournamentStatus.Draft,
  });

  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateTournamentDto, string>>
  >({});

  const updateField = <K extends keyof CreateTournamentDto>(
    field: K,
    value: CreateTournamentDto[K],
  ) => {
    setFormData((current) => ({ ...current, [field]: value }));
    setErrors((current) => ({ ...current, [field]: undefined }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = createTournamentDtoSchema.safeParse(formData);

    if (!result.success) {
      const fieldErrors: Partial<Record<keyof CreateTournamentDto, string>> =
        {};

      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof CreateTournamentDto;

        if (!fieldErrors[field]) {
          fieldErrors[field] = issue.message;
        }
      });

      setErrors(fieldErrors);
      return;
    }

    setErrors({});

    const tournament = await handleAdd(result.data);

    if (tournament) {
      navigate("/tournaments");
    }
  };

  const inputClass = (field: keyof CreateTournamentDto) =>
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
            Turnier erstellen
          </h1>
          <p className="mt-2 text-sm text-gray-600">
            Erstelle ein neues Turnier und lege die grundlegenden Einstellungen
            fest.
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
                    Turniername
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
                  placeholder="z. B. DartForge Sommer Cup"
                />

                {errors.name && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.name}</p>
                )}
              </div>

              {/* Start date */}
              <div>
                <label
                  htmlFor="startDate"
                  className="block text-sm font-medium text-gray-900"
                >
                  Startdatum und -zeit
                </label>

                <input
                  id="startDate"
                  type="datetime-local"
                  value={startDateInput}
                  onChange={(event) => {
                    const value = event.target.value;
                    setStartDateInput(value);
                    updateField(
                      "startDate",
                      value ? new Date(value).toISOString() : "",
                    );
                  }}
                  disabled={adding}
                  className={inputClass("startDate")}
                />

                {errors.startDate && (
                  <p className="mt-1.5 text-sm text-red-600">
                    {errors.startDate}
                  </p>
                )}
              </div>

              {/* Description */}
              <div>
                <div className="flex items-center justify-between">
                  <label
                    htmlFor="description"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Beschreibung
                  </label>
                  <span className="text-xs text-gray-400">
                    max. 500 Zeichen
                  </span>
                </div>

                <textarea
                  id="description"
                  rows={4}
                  maxLength={500}
                  value={formData.description ?? ""}
                  onChange={(event) =>
                    updateField("description", event.target.value || null)
                  }
                  disabled={adding}
                  className={inputClass("description")}
                  placeholder="Optionale Informationen zum Turnier"
                />

                {errors.description && (
                  <p className="mt-1.5 text-sm text-red-600">
                    {errors.description}
                  </p>
                )}
              </div>

              {/* Mode */}
              <div>
                <label className="block text-sm font-medium text-gray-900">
                  Turniermodus
                </label>

                <div className="mt-2 space-y-3">
                  {modeOptions.map((option) => (
                    <label
                      key={option.value}
                      className={`flex cursor-pointer gap-3 rounded-lg border p-4 transition ${
                        formData.mode === option.value
                          ? "border-blue-500 bg-blue-50/50 ring-1 ring-blue-500"
                          : "border-gray-200 bg-white hover:bg-gray-50"
                      } ${adding ? "cursor-not-allowed opacity-60" : ""}`}
                    >
                      <input
                        type="radio"
                        name="mode"
                        value={option.value}
                        checked={formData.mode === option.value}
                        onChange={() => updateField("mode", option.value)}
                        disabled={adding}
                        className="mt-0.5 h-4 w-4 border-gray-300 text-blue-600 focus:ring-2 focus:ring-blue-500"
                      />
                      <span>
                        <span className="block text-sm font-medium text-gray-900">
                          {option.label}
                        </span>
                        <span className="mt-1 block text-xs text-gray-500">
                          {option.description}
                        </span>
                      </span>
                    </label>
                  ))}
                </div>

                {errors.mode && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.mode}</p>
                )}
              </div>

              {/* Status */}
              <div>
                <label
                  htmlFor="status"
                  className="block text-sm font-medium text-gray-900"
                >
                  Status
                </label>

                <select
                  id="status"
                  value={formData.status}
                  onChange={(event) =>
                    updateField(
                      "status",
                      Number(event.target.value) as TournamentStatus,
                    )
                  }
                  disabled={adding}
                  className={inputClass("status")}
                >
                  {statusOptions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>

                {errors.status && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.status}</p>
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
                {adding ? "Speichern..." : "Turnier erstellen"}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
