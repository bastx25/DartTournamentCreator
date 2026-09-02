import { useState } from "react";
import type { CreatePlayer } from "../../types/CreatePlayer";

interface CreatePlayerModalProps {
  adding: boolean;
  onCancel: () => void;
  onConfirm: (createdPlayer: CreatePlayer) => void;
}

export function CreatePlayerModal({
  adding,
  onCancel,
  onConfirm,
}: CreatePlayerModalProps) {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [nickname, setNickname] = useState("");

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const updatedPlayer: CreatePlayer = {
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      nickname: nickname.trim(),
    };

    onConfirm(updatedPlayer);
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
        aria-labelledby="update-player-title"
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
              id="update-player-title"
              className="mt-4 text-lg font-semibold text-gray-900"
            >
              Spieler bearbeiten
            </h3>

            <p className="mt-2 text-sm leading-6 text-gray-500">
              Ändere die Daten des Spielers und speichere anschließend die
              Änderungen.
            </p>

            {/* Form fields */}
            <div className="mt-6 space-y-4">
              <div>
                <label
                  htmlFor="firstName"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Vorname
                </label>

                <input
                  id="firstName"
                  type="text"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  disabled={adding}
                  className="w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
                  placeholder="Vorname"
                />
              </div>

              <div>
                <label
                  htmlFor="lastName"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Nachname
                </label>

                <input
                  id="lastName"
                  type="text"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  disabled={adding}
                  className="w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
                  placeholder="Nachname"
                />
              </div>

              <div>
                <label
                  htmlFor="nickname"
                  className="mb-1.5 block text-sm font-medium text-gray-700"
                >
                  Spitzname
                </label>

                <input
                  id="nickname"
                  type="text"
                  value={nickname}
                  onChange={(e) => setNickname(e.target.value)}
                  disabled={adding}
                  className="w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 disabled:cursor-not-allowed disabled:bg-gray-100"
                  placeholder="Spitzname"
                />
              </div>
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

              {adding ? "Speichern..." : "Änderungen speichern"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
