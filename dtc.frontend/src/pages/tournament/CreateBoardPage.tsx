import { useState } from "react";
import { useNavigate } from "react-router";
import Header from "../../components/Header";
import {
  createBoardDtoSchema,
  type CreateBoardDto,
} from "../../dtos/board/CreateBoardDto";
import { createBoard } from "../../services/boardService";

export function CreateBoardPage() {
  const [formData, setFormData] = useState<CreateBoardDto>({
    locationId: 0,
    number: 1,
    label: null,
    isActive: true,
  });

  const navigate = useNavigate();
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<
    Partial<Record<keyof CreateBoardDto, string>>
  >({});

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

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
    setSaving(true);

    try {
      const created = await createBoard(result.data);
      navigate(`/boards/${created.id}/qr`);
    } catch {
      setErrors({
        label: "Board konnte nicht gespeichert werden. Bitte API/Verbindung prüfen.",
      });
    } finally {
      setSaving(false);
    }
  };

  const inputClass = (field: keyof CreateBoardDto) =>
    `
      mt-2 block w-full rounded-lg border bg-white px-3 py-2.5
      text-sm text-gray-900 shadow-sm outline-none
      transition
      placeholder:text-gray-400
      focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20
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
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold tracking-tight text-gray-900">
            Board erstellen
          </h1>

          <p className="mt-2 text-sm text-gray-600">
            Erstelle ein neues Board und lege die grundlegenden Einstellungen
            fest.
          </p>
        </div>

        {/* Card */}
        <div className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
          <form onSubmit={handleSubmit}>
            <div className="space-y-6 p-6 sm:p-8">
              {/* Location ID */}
              <div>
                <label
                  htmlFor="locationId"
                  className="block text-sm font-medium text-gray-900"
                >
                  Location ID
                </label>

                <input
                  id="locationId"
                  type="number"
                  value={formData.locationId}
                  onChange={(event) => {
                    setFormData({
                      ...formData,
                      locationId: Number(event.target.value),
                    });

                    setErrors({
                      ...errors,
                      locationId: undefined,
                    });
                  }}
                  className={inputClass("locationId")}
                  placeholder="z. B. 1"
                />

                {errors.locationId && (
                  <p className="mt-1.5 text-sm text-red-600">
                    {errors.locationId}
                  </p>
                )}
              </div>

              {/* Board Number */}
              <div>
                <label
                  htmlFor="number"
                  className="block text-sm font-medium text-gray-900"
                >
                  Boardnummer
                </label>

                <p className="mt-1 text-xs text-gray-500">
                  Die Boardnummer muss zwischen 1 und 100 liegen.
                </p>

                <input
                  id="number"
                  type="number"
                  min={1}
                  max={100}
                  value={formData.number}
                  onChange={(event) => {
                    setFormData({
                      ...formData,
                      number: Number(event.target.value),
                    });

                    setErrors({
                      ...errors,
                      number: undefined,
                    });
                  }}
                  className={inputClass("number")}
                  placeholder="z. B. 1"
                />

                {errors.number && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.number}</p>
                )}
              </div>

              {/* Label */}
              <div>
                <div className="flex items-center justify-between">
                  <label
                    htmlFor="label"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Label
                  </label>

                  <span className="text-xs text-gray-400">max. 50 Zeichen</span>
                </div>

                <input
                  id="label"
                  type="text"
                  maxLength={50}
                  value={formData.label ?? ""}
                  onChange={(event) => {
                    setFormData({
                      ...formData,
                      label: event.target.value || null,
                    });

                    setErrors({
                      ...errors,
                      label: undefined,
                    });
                  }}
                  className={inputClass("label")}
                  placeholder="z. B. Hauptplatz"
                />

                {errors.label && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.label}</p>
                )}
              </div>

              {/* Active */}
              <div className="rounded-lg border border-gray-200 bg-gray-50 p-4">
                <label
                  htmlFor="isActive"
                  className="flex cursor-pointer items-center gap-3"
                >
                  <input
                    id="isActive"
                    type="checkbox"
                    checked={formData.isActive}
                    onChange={(event) => {
                      setFormData({
                        ...formData,
                        isActive: event.target.checked,
                      });
                    }}
                    className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-2 focus:ring-blue-500"
                  />

                  <div>
                    <span className="block text-sm font-medium text-gray-900">
                      Board aktiv
                    </span>

                    <span className="block text-xs text-gray-500">
                      Das Board kann direkt verwendet werden.
                    </span>
                  </div>
                </label>
              </div>
            </div>

            {/* Footer */}
            <div className="flex flex-col-reverse gap-3 border-t border-gray-200 bg-gray-50 px-6 py-4 sm:flex-row sm:justify-end sm:px-8">
              <button
                type="button"
                className="rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 shadow-sm transition hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
                onClick={() => window.history.back()}
              >
                Abbrechen
              </button>

              <button
                type="submit"
                disabled={saving}
                className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/40 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {saving ? "Board wird erstellt..." : "Board erstellen"}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
