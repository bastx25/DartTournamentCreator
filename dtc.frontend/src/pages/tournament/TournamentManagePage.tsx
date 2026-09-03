import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Header from "../../components/Header";
import { getPlayers } from "../../services/playerService";

import type { TournamentDto } from "../../dtos/Tournament/TournamentDto";
import type { PlayerDto } from "../../dtos/Player/PlayerDto";
import {
  generateGroups,
  generateKnockout,
  getTournament,
} from "../../services/tournamentService";
import { RoundPhase } from "../../enums/RoundPhase";

function formatDate(value: string) {
  return new Intl.DateTimeFormat("de-DE", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function TournamentManagePage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const tournamentId = Number(id);

  const [tournament, setTournament] = useState<TournamentDto | null>(null);
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [selectedPlayerIds, setSelectedPlayerIds] = useState<number[]>([]);
  const [groupCount, setGroupCount] = useState(2);
  const [groupSize, setGroupSize] = useState(4);
  const [qualifiersPerGroup, setQualifiersPerGroup] = useState(2);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const isValidTournamentId =
    Number.isInteger(tournamentId) && tournamentId > 0;

  useEffect(() => {
    if (!isValidTournamentId) {
      return;
    }

    async function load() {
      try {
        setLoading(true);
        setError(null);

        const [tournamentData, playerData] = await Promise.all([
          getTournament(tournamentId),
          getPlayers(),
        ]);

        setTournament(tournamentData);
        setPlayers(playerData);
        setSelectedPlayerIds(playerData.map((player) => player.id));
      } catch (err) {
        console.error(err);
        setError("Die Turnierdaten konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    void load();
  }, [tournamentId, isValidTournamentId]);

  if (!isValidTournamentId) {
    return <div>Ungültige Turnier-ID.</div>;
  }

  const selectedPlayers = selectedPlayerIds.length;

  const togglePlayer = (playerId: number) => {
    setSelectedPlayerIds((current) =>
      current.includes(playerId)
        ? current.filter((id) => id !== playerId)
        : [...current, playerId],
    );
    setSuccess(null);
  };

  const handleGenerate = async () => {
    if (
      selectedPlayers === 0 ||
      groupCount < 1 ||
      groupSize < 2 ||
      selectedPlayers < groupCount * 2 ||
      selectedPlayers > groupCount * groupSize ||
      qualifiersPerGroup < 1 ||
      qualifiersPerGroup > groupSize
    ) {
      setError("Prüfe Spieleranzahl, Gruppengröße, Gruppenzahl und Weiterkommer.");
      return;
    }

    try {
      setGenerating(true);
      setError(null);
      setSuccess(null);

      console.log("matches werden erstellt");

      await generateGroups(tournamentId, {
        groupCount,
        groupSize,
        qualifiersPerGroup,
        startTime: tournament?.startDate ?? null,
        matchDurationMinutes: tournament?.matchDurationMinutes ?? null,
        breakBetweenMatchesMinutes: tournament?.breakBetweenMatchesMinutes ?? null,
        playerIds: selectedPlayerIds,
      });

      const updated = await getTournament(tournamentId);
      setTournament(updated);
      setSuccess("Gruppen und Matches wurden erfolgreich erstellt.");
    } catch (err) {
      console.error(err);
      setError("Gruppen und Matches konnten nicht erstellt werden.");
    } finally {
      setGenerating(false);
    }
  };

  const knockoutPrepared =
    tournament?.rounds.some((round) => round.phase === RoundPhase.Knockout) ?? false;
  const knockoutGenerated =
    tournament?.rounds
      .filter((round) => round.phase === RoundPhase.Knockout)
      .some((round) => round.matches.some((match) => match.participants.length > 0)) ??
    false;

  const handleGenerateKnockout = async () => {
    try {
      setGenerating(true);
      setError(null);
      setSuccess(null);
      await generateKnockout(tournamentId);
      setTournament(await getTournament(tournamentId));
      setSuccess("Die K.-o.-Phase wurde aus den Match-Siegen generiert.");
    } catch (err) {
      console.error(err);
      setError("Die K.-o.-Phase konnte nicht generiert werden.");
    } finally {
      setGenerating(false);
    }
  };

  if (loading) {
    return (
      <>
        <Header />
        <main className="min-h-screen bg-gray-50 px-4 py-10 text-center text-sm text-gray-400">
          Turnier wird geladen...
        </main>
      </>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <main className="mx-auto w-full max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <button
              type="button"
              onClick={() => navigate("/tournaments")}
              className="mb-3 text-sm font-medium text-blue-600 hover:text-blue-700"
            >
              ← Zurück zu den Turnieren
            </button>
            <h1 className="text-3xl font-bold tracking-tight text-gray-900">
              {tournament?.name ?? "Turnier verwalten"}
            </h1>
            {tournament && (
              <p className="mt-2 text-sm text-gray-600">
                Start: {formatDate(tournament.startDate)}
                {tournament.description ? ` · ${tournament.description}` : ""}
              </p>
            )}
          </div>
        </div>

        {(error || success) && (
          <div
            className={`mb-6 rounded-lg border p-4 ${
              error
                ? "border-red-500/20 bg-red-500/10"
                : "border-green-500/20 bg-green-500/10"
            }`}
          >
            <p
              className={`text-sm ${error ? "text-red-600" : "text-green-700"}`}
            >
              {error ?? success}
            </p>
          </div>
        )}

        <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_360px]">
          <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
            <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
              <h2 className="text-lg font-semibold text-gray-900">Spieler</h2>
              <p className="mt-1 text-sm text-gray-500">
                Lege fest, welche Spieler an der Gruppenphase teilnehmen.
              </p>
            </div>

            <div className="flex items-center justify-between border-b border-gray-100 bg-gray-50 px-6 py-3 sm:px-8">
              <span className="text-sm font-medium text-gray-700">
                {selectedPlayers} von {players.length} ausgewählt
              </span>
              <div className="flex gap-3">
                <button
                  type="button"
                  onClick={() => setSelectedPlayerIds(players.map((p) => p.id))}
                  className="text-xs font-medium text-blue-600 hover:text-blue-700"
                >
                  Alle auswählen
                </button>
                <button
                  type="button"
                  onClick={() => setSelectedPlayerIds([])}
                  className="text-xs font-medium text-gray-500 hover:text-gray-700"
                >
                  Keine
                </button>
              </div>
            </div>

            <div className="divide-y divide-gray-100">
              {players.map((player) => {
                const selected = selectedPlayerIds.includes(player.id);
                return (
                  <label
                    key={player.id}
                    className="flex cursor-pointer items-center gap-3 px-6 py-3 transition hover:bg-gray-50 sm:px-8"
                  >
                    <input
                      type="checkbox"
                      checked={selected}
                      onChange={() => togglePlayer(player.id)}
                      className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                    />
                    <span className="min-w-0">
                      <span className="block text-sm font-medium text-gray-900">
                        {player.displayName}
                      </span>
                      {player.nickname && (
                        <span className="block text-xs text-gray-500">
                          {player.nickname}
                        </span>
                      )}
                    </span>
                  </label>
                );
              })}

              {players.length === 0 && (
                <div className="px-8 py-10 text-center text-sm text-gray-500">
                  Es sind noch keine Spieler vorhanden.
                </div>
              )}
            </div>
          </section>

          <aside className="space-y-6">
            <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
              <div className="border-b border-gray-200 px-6 py-5">
                <h2 className="text-lg font-semibold text-gray-900">
                  Gruppen erstellen
                </h2>
                <p className="mt-1 text-sm text-gray-500">
                  Die ausgewählten Spieler werden zufällig auf Gruppen verteilt.
                </p>
              </div>

              <div className="space-y-5 p-6">
                <div>
                  <label
                    htmlFor="groupCount"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Anzahl der Gruppen
                  </label>
                  <input
                    id="groupCount"
                    type="number"
                    min={1}
                    max={Math.max(1, selectedPlayers)}
                    value={groupCount}
                    onChange={(event) =>
                      setGroupCount(
                        Math.max(1, Number(event.target.value) || 1),
                      )
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                  <p className="mt-2 text-xs text-gray-500">
                    Maximal {groupSize} Spieler pro Gruppe. Die tatsächliche Verteilung wird so ausgeglichen wie möglich vorgenommen.
                  </p>
                </div>

                <div>
                  <label
                    htmlFor="groupSize"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Max. Spieler pro Gruppe
                  </label>
                  <input
                    id="groupSize"
                    type="number"
                    min={2}
                    max={64}
                    value={groupSize}
                    onChange={(event) =>
                      setGroupSize(Math.max(2, Number(event.target.value) || 2))
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                </div>

                <div>
                  <label
                    htmlFor="qualifiersPerGroup"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Weiterkommer pro Gruppe
                  </label>
                  <input
                    id="qualifiersPerGroup"
                    type="number"
                    min={1}
                    max={Math.max(1, groupSize)}
                    value={qualifiersPerGroup}
                    onChange={(event) =>
                      setQualifiersPerGroup(
                        Math.max(1, Number(event.target.value) || 1),
                      )
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                </div>

                <div className="rounded-lg border border-blue-100 bg-blue-50 p-4">
                  <p className="text-xs font-medium uppercase tracking-wide text-blue-700">
                    Aktuelle Konfiguration
                  </p>
                  <dl className="mt-3 space-y-2 text-sm">
                    <div className="flex justify-between gap-4">
                      <dt className="text-gray-600">Spieler</dt>
                      <dd className="font-medium text-gray-900">
                        {selectedPlayers}
                      </dd>
                    </div>
                    <div className="flex justify-between gap-4">
                      <dt className="text-gray-600">Gruppen</dt>
                      <dd className="font-medium text-gray-900">
                        {groupCount}
                      </dd>
                    </div>
                    <div className="flex justify-between gap-4">
                      <dt className="text-gray-600">Max. Spieler / Gruppe</dt>
                      <dd className="font-medium text-gray-900">{groupSize}</dd>
                    </div>
                    <div className="flex justify-between gap-4">
                      <dt className="text-gray-600">Weiterkommer / Gruppe</dt>
                      <dd className="font-medium text-gray-900">{qualifiersPerGroup}</dd>
                    </div>
                  </dl>
                </div>

                <button
                  type="button"
                  disabled={generating || selectedPlayers < 2}
                  onClick={() => void handleGenerate()}
                  className="inline-flex w-full items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/40 disabled:cursor-not-allowed disabled:opacity-60"
                >
                  {generating
                    ? "Gruppen werden erstellt..."
                    : "Zufällig Gruppen & Matches erstellen"}
                </button>
                {knockoutPrepared && tournament?.mode === 1 && (
                  <button
                    type="button"
                    disabled={generating || knockoutGenerated}
                    onClick={() => void handleGenerateKnockout()}
                    className="inline-flex w-full items-center justify-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-800 shadow-sm transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-60"
                  >
                    {knockoutGenerated
                      ? "K.-o.-Phase bereits generiert"
                      : "KO-Phase generieren"}
                  </button>
                )}

              </div>
            </section>

            <section className="rounded-xl border border-dashed border-gray-300 bg-white p-6 shadow-sm">
              <h2 className="text-lg font-semibold text-gray-900">
                Manuelle Gruppenzuteilung
              </h2>
              <p className="mt-1 text-sm text-gray-500">
                Spieler später per Gruppe zuordnen und die Zusammensetzung vor
                der Match-Erstellung bearbeiten.
              </p>
              <div className="mt-4 rounded-lg bg-gray-50 p-4 text-xs text-gray-500">
                Dieser Bereich wird im nächsten Schritt mit der manuellen
                Gruppenzuteilung verbunden.
              </div>
            </section>
          </aside>
        </div>

        {tournament && tournament.rounds.length > 0 && (
          <section className="mt-6 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
            <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
              <h2 className="text-lg font-semibold text-gray-900">
                Erstellte Runden & Matches
              </h2>
              <p className="mt-1 text-sm text-gray-500">
                Die aktuell vom MatchMaker erzeugten Daten.
              </p>
            </div>
            <div className="divide-y divide-gray-100">
              {tournament.rounds.map((round) => (
                <div
                  key={round.id}
                  className="flex flex-col gap-2 px-6 py-4 sm:flex-row sm:items-center sm:justify-between sm:px-8"
                >
                  <div>
                    <p className="text-sm font-medium text-gray-900">
                      {round.name ?? `Runde ${round.sequence}`}
                    </p>
                    <p className="mt-1 text-xs text-gray-500">
                      {round.matches.length}{" "}
                      {round.matches.length === 1 ? "Match" : "Matches"}
                    </p>
                  </div>
                  <span className="text-xs text-gray-500">
                    {formatDate(round.plannedStart)}
                  </span>
                </div>
              ))}
            </div>
          </section>
        )}
      </main>
    </div>
  );
}
