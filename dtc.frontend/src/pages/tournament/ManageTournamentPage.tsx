import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import Header from "../../components/Header";
import { getPlayers } from "../../services/playerService";

import {
  deleteTournament,
  generateGroups,
  generateKnockout,
  getTConfigsByTournamentId,
  getTournament,
  getTPlayers,
  saveManualGroups,
} from "../../services/tournamentService";
import { TournamentMode } from "../../enums/TournamentMode";
import { KnockoutDrawMode } from "../../enums/KnockoutDrawMode";
import { DeleteTournamentModal } from "./DeleteTournamentModal";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import type { PlayerDto } from "../../dtos/player/PlayerDto";
import type { TournamentConfigDto } from "../../dtos/tournamentConfig/TournamentConfigDto";
import type { TournamentPlayerDto } from "../../dtos/tournamentPlayer/TournamentPlayerDto";
import type { ManualGroupItemDto } from "../../dtos/matchMaker/ManualGroupDto";

function getGroupName(index: number): string {
  let value = index + 1;
  let result = "";
  while (value > 0) {
    value--;
    result = String.fromCharCode(65 + (value % 26)) + result;
    value = Math.floor(value / 26);
  }
  return `Gruppe ${result}`;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("de-DE", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function ManageTournamentPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const tournamentId = Number(id);

  const [tournament, setTournament] = useState<TournamentDto | null>(null);
  const [tournamentToDelete, setTournamentToDelete] =
    useState<TournamentDto | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [selectedPlayerIds, setSelectedPlayerIds] = useState<number[]>([]);
  const [groupCount, setGroupCount] = useState(2);
  const groupSize = 5;
  const [qualifiersPerGroup, setQualifiersPerGroup] = useState(2);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [breakBetweenMatchesMinutes, setBreakBetweenMatchesMinutes] =
    useState(0);
  const [koPhaseStartTime, setKoPhaseStartTime] = useState("");
  const [matchDurationMinutes, setMatchDurationMinutes] = useState(0);
  const [knockoutDrawMode, setKnockoutDrawMode] = useState<KnockoutDrawMode>(
    KnockoutDrawMode.Random,
  );

  const [groupMode, setGroupMode] = useState<"auto" | "manual">("auto");
  const [manualGroups, setManualGroups] = useState<ManualGroupItemDto[]>([
    { name: "Gruppe A", playerIds: [], qualifiersCount: 2 },
    { name: "Gruppe B", playerIds: [], qualifiersCount: 2 },
  ]);

  const assignedPlayerIds = manualGroups.flatMap((g) => g.playerIds);
  const unassignedPlayers = players.filter(
    (p) => selectedPlayerIds.includes(p.id) && !assignedPlayerIds.includes(p.id),
  );

  const handleAddManualGroup = () => {
    const nextIndex = manualGroups.length;
    setManualGroups((prev) => [
      ...prev,
      {
        name: getGroupName(nextIndex),
        playerIds: [],
        qualifiersCount: qualifiersPerGroup || 2,
      },
    ]);
  };

  const handleRemoveManualGroup = (index: number) => {
    setManualGroups((prev) => prev.filter((_, i) => i !== index));
  };

  const handleRenameManualGroup = (index: number, newName: string) => {
    setManualGroups((prev) =>
      prev.map((g, i) => (i === index ? { ...g, name: newName } : g)),
    );
  };

  const handleGroupQualifiersChange = (index: number, count: number) => {
    setManualGroups((prev) =>
      prev.map((g, i) =>
        i === index ? { ...g, qualifiersCount: Math.max(1, count) } : g,
      ),
    );
  };

  const handleAssignPlayerToGroup = (
    playerId: number,
    targetGroupIndex: number,
  ) => {
    setManualGroups((prev) =>
      prev.map((g, i) => {
        const withoutPlayer = g.playerIds.filter((id) => id !== playerId);
        if (i === targetGroupIndex) {
          return { ...g, playerIds: [...withoutPlayer, playerId] };
        }
        return { ...g, playerIds: withoutPlayer };
      }),
    );
  };

  const handleRemovePlayerFromManualGroup = (playerId: number) => {
    setManualGroups((prev) =>
      prev.map((g) => ({
        ...g,
        playerIds: g.playerIds.filter((id) => id !== playerId),
      })),
    );
  };

  const handleSaveManual = async () => {
    if (manualGroups.length === 0) {
      setError("Erstelle bitte mindestens eine Gruppe.");
      return;
    }

    for (const grp of manualGroups) {
      if (!grp.name.trim()) {
        setError("Jede Gruppe muss einen Namen haben.");
        return;
      }
      if (grp.playerIds.length < 2) {
        setError(
          `Gruppe "${grp.name}" benötigt mindestens 2 Spieler für Matches.`,
        );
        return;
      }
    }

    try {
      setGenerating(true);
      setError(null);
      setSuccess(null);

      await saveManualGroups(tournamentId, {
        groups: manualGroups,
        startTime: tournament?.startDate ?? new Date().toISOString(),
        matchDurationMinutes,
        breakBetweenMatchesMinutes,
        knockoutDrawMode,
      });

      const updated = await getTournament(tournamentId);
      setTournament(updated);
      setSuccess("Manuelle Gruppen und Matches wurden erfolgreich erstellt.");
    } catch (err: unknown) {
      console.error(err);
      setError("Manuelle Gruppen konnten nicht erstellt werden.");
    } finally {
      setGenerating(false);
    }
  };

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

        const [tournamentData, playerData, tPlayerData] = await Promise.all([
          getTournament(tournamentId),
          getPlayers(),
          getTPlayers(tournamentId),
        ]);

        setTournament(tournamentData);
        setPlayers(playerData);

        setSelectedPlayerIds(
          tPlayerData.map((p: TournamentPlayerDto) => p.playerId),
        );
      } catch (err) {
        console.error(err);
        setError("Die Turnierdaten konnten nicht geladen werden.");
      } finally {
        setLoading(false);
      }
    }

    void load();
  }, [tournamentId, isValidTournamentId]);

  const [tconfigs, setConfigs] = useState<TournamentConfigDto[]>([]);

  useEffect(() => {
    if (tournament === null) return;

    const loadConfig = async () => {
      const response = await getTConfigsByTournamentId(tournament.id);
      setConfigs(response);
    };

    loadConfig();
  }, [tournament]);

  const [selectedTConfig, setSelectedTConfig] =
    useState<TournamentConfigDto | null>();
  useEffect(() => {
    const selectTConfig = async () => {
      setSelectedTConfig(tconfigs.length > 0 ? tconfigs[0] : null);
    };

    selectTConfig();
  }, [tconfigs]);

  useEffect(() => {
    const configFields = async () => {
      setBreakBetweenMatchesMinutes(
        selectedTConfig?.breakBetweenMatchesMinutes ?? 0,
      );

      setMatchDurationMinutes(selectedTConfig?.matchDurationMinutes ?? 15);
      setGroupCount(selectedTConfig?.groupCount ?? 2);
      setQualifiersPerGroup(selectedTConfig?.qualifiersPerGroup ?? 2);
      setKnockoutDrawMode(
        selectedTConfig?.knockoutDrawMode ?? KnockoutDrawMode.Random,
      );
    };

    configFields();
  }, [selectedTConfig]);

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
    if (selectedPlayers === 0 || groupCount < 1) {
      setError("Prüfe Spieleranzahl oder Gruppenzahl.");
      return;
    }

    // if (
    //   !isPowerOfTwo(qualifiersPerGroup * groupCount) &&
    //   selectedTConfig?.mode == TournamentMode.GroupStageandKnockout
    // ) {
    //   setError(
    //     "Es kann keine Ko-Phase genertiert werden, da die Gruppen oder Weiterkommenden Spieler keine 2er Potenz ergeben",
    //   );
    //   return;
    // }

    try {
      setGenerating(true);
      setError(null);
      setSuccess(null);

      console.log("matches werden erstellt");

      await generateGroups(tournamentId, {
        groupCount,
        playersPerGroup: groupSize,
        qualifiersPerGroup,
        startTime: tournament?.startDate ?? new Date().toISOString(),
        matchDurationMinutes: matchDurationMinutes,
        breakBetweenMatchesMinutes: breakBetweenMatchesMinutes,
        playerIds: selectedPlayerIds,
        knockoutDrawMode,
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

  const knockoutPrepared = true;
  // brakets?.some((braket) => braket.phase === BraketPhase.Knockout) ?? false;

  const knockoutGenerated = false;
  // brakets
  //   ?.filter((braket) => braket.phase === BraketPhase.Knockout)
  //   .some((braket) => braket.matches.length > 0) ?? false;

  const handleGenerateKnockout = async () => {
    try {
      setGenerating(true);
      setError(null);
      setSuccess(null);
      await generateKnockout(tournamentId, {
        groupCount,
        playersPerGroup: groupSize,
        qualifiersPerGroup,
        startTime: koPhaseStartTime,
        matchDurationMinutes: matchDurationMinutes,
        breakBetweenMatchesMinutes: breakBetweenMatchesMinutes,
        playerIds: selectedPlayerIds,
        knockoutDrawMode,
      });

      setTournament(await getTournament(tournamentId));
      setSuccess("Die K.-o.-Phase wurde aus den Match-Siegen generiert.");
    } catch (err) {
      console.error(err);
      setError("Die K.-o.-Phase konnte nicht generiert werden.");
    } finally {
      setGenerating(false);
    }
  };

  const handleDelete = async () => {
    try {
      setDeleting(true);
      if (tournament) {
        await deleteTournament(tournament.id);
      }

      navigate("/tournaments");
    } finally {
      setDeleting(false);
      setTournamentToDelete(null);
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
            {tournament?.code && (
              <div className="mt-3 flex flex-wrap items-center gap-3">
                <div className="flex items-center gap-2 rounded-lg border border-blue-200 bg-blue-50 px-3 py-1.5 text-sm text-blue-900">
                  <span className="text-xs font-medium text-blue-700">Turnier-Code:</span>
                  <span className="font-mono font-bold">{tournament.code}</span>
                  <button
                    type="button"
                    onClick={() => {
                      if (tournament.code) {
                        navigator.clipboard.writeText(tournament.code);
                      }
                    }}
                    className="ml-1 text-xs text-blue-600 hover:text-blue-800 underline"
                  >
                    Code kopieren
                  </button>
                </div>

                <button
                  type="button"
                  onClick={() => {
                    if (tournament.code) {
                      const shareUrl = `${window.location.origin}/dashboard?code=${tournament.code}`;
                      navigator.clipboard.writeText(shareUrl);
                    }
                  }}
                  className="rounded-lg border border-gray-300 bg-white px-3 py-1.5 text-xs font-semibold text-gray-700 hover:bg-gray-50 transition shadow-sm"
                >
                  🔗 Öffentlichen Dashboard-Link kopieren
                </button>

                <button
                  type="button"
                  onClick={() => navigate(`/dashboard?code=${tournament.code}`)}
                  className="rounded-lg border border-gray-300 bg-white px-3 py-1.5 text-xs font-semibold text-gray-700 hover:bg-gray-50 transition shadow-sm"
                >
                  📊 Im Dashboard öffnen
                </button>
              </div>
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

        {/* Modus-Umschaltung: Automatisch vs. Manuell */}
        <div className="mb-6 flex max-w-md gap-2 rounded-xl border border-gray-200 bg-white p-1.5 shadow-sm">
          <button
            type="button"
            onClick={() => setGroupMode("auto")}
            className={`flex-1 rounded-lg py-2 text-center text-sm font-semibold transition ${
              groupMode === "auto"
                ? "bg-blue-600 text-white shadow-sm"
                : "text-gray-600 hover:bg-gray-50 hover:text-gray-900"
            }`}
          >
            🎲 Automatische Einteilung
          </button>
          <button
            type="button"
            onClick={() => setGroupMode("manual")}
            className={`flex-1 rounded-lg py-2 text-center text-sm font-semibold transition ${
              groupMode === "manual"
                ? "bg-blue-600 text-white shadow-sm"
                : "text-gray-600 hover:bg-gray-50 hover:text-gray-900"
            }`}
          >
            ✍️ Manuelle Einteilung
          </button>
        </div>

        <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_360px]">
          {groupMode === "auto" ? (
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
          ) : (
            <div className="space-y-6">
              {/* Manuelle Gruppen Zusammenstellung */}
              <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
                <div className="flex flex-col gap-3 border-b border-gray-200 px-6 py-5 sm:flex-row sm:items-center sm:justify-between sm:px-8">
                  <div>
                    <h2 className="text-lg font-semibold text-gray-900">
                      Gruppen konfigurieren ({manualGroups.length})
                    </h2>
                    <p className="mt-1 text-sm text-gray-500">
                      Erstelle Gruppen, benenne sie und weise ihnen Spieler zu.
                    </p>
                  </div>
                  <button
                    type="button"
                    onClick={handleAddManualGroup}
                    className="inline-flex items-center gap-1.5 rounded-lg bg-blue-50 px-3 py-2 text-xs font-semibold text-blue-700 transition hover:bg-blue-100"
                  >
                    <span>+</span> Neue Gruppe hinzufügen
                  </button>
                </div>

                <div className="grid gap-4 p-6 sm:grid-cols-2">
                  {manualGroups.map((group, groupIdx) => (
                    <div
                      key={groupIdx}
                      className="flex flex-col rounded-xl border border-gray-200 bg-gray-50/50 p-4"
                    >
                      <div className="mb-3 flex items-center justify-between gap-2">
                        <input
                          type="text"
                          value={group.name}
                          onChange={(e) =>
                            handleRenameManualGroup(groupIdx, e.target.value)
                          }
                          className="w-full rounded-md border border-gray-300 bg-white px-2.5 py-1 text-sm font-bold text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
                          placeholder="Gruppenname"
                        />
                        <button
                          type="button"
                          onClick={() => handleRemoveManualGroup(groupIdx)}
                          className="rounded-md p-1 text-gray-400 hover:bg-red-50 hover:text-red-600"
                          title="Gruppe löschen"
                        >
                          ✕
                        </button>
                      </div>

                      <div className="mb-3 flex items-center justify-between text-xs text-gray-500">
                        <div className="flex items-center gap-1.5">
                          <span>Weiterkommer:</span>
                          <input
                            type="number"
                            min={1}
                            max={Math.max(1, group.playerIds.length)}
                            value={group.qualifiersCount}
                            onChange={(e) =>
                              handleGroupQualifiersChange(
                                groupIdx,
                                Number(e.target.value) || 1,
                              )
                            }
                            className="w-12 rounded border border-gray-300 bg-white px-1.5 py-0.5 text-center text-xs text-gray-900"
                          />
                        </div>
                        <span className="font-semibold text-gray-700">
                          {group.playerIds.length} Spieler
                        </span>
                      </div>

                      {/* Spielerliste in der Gruppe */}
                      <div className="min-h-24 flex-1 space-y-1.5 rounded-lg border border-gray-200 bg-white p-2">
                        {group.playerIds.length === 0 ? (
                          <div className="flex h-full min-h-20 items-center justify-center text-center text-xs text-gray-400">
                            Noch keine Spieler zugewiesen
                          </div>
                        ) : (
                          group.playerIds.map((playerId) => {
                            const p = players.find((pl) => pl.id === playerId);
                            return (
                              <div
                                key={playerId}
                                className="flex items-center justify-between rounded-md bg-gray-50 px-2.5 py-1.5 text-xs transition hover:bg-gray-100"
                              >
                                <span className="truncate font-medium text-gray-900">
                                  {p?.displayName ?? `Spieler ${playerId}`}
                                </span>
                                <div className="flex items-center gap-1.5">
                                  <select
                                    value={groupIdx}
                                    onChange={(e) =>
                                      handleAssignPlayerToGroup(
                                        playerId,
                                        Number(e.target.value),
                                      )
                                    }
                                    className="rounded border border-gray-300 bg-white px-1.5 py-0.5 text-[11px] text-gray-600 outline-none"
                                    title="In andere Gruppe verschieben"
                                  >
                                    {manualGroups.map((otherG, otherIdx) => (
                                      <option key={otherIdx} value={otherIdx}>
                                        {otherIdx === groupIdx
                                          ? "Verschieben..."
                                          : `→ ${otherG.name}`}
                                      </option>
                                    ))}
                                  </select>
                                  <button
                                    type="button"
                                    onClick={() =>
                                      handleRemovePlayerFromManualGroup(playerId)
                                    }
                                    className="rounded p-0.5 text-gray-400 hover:text-red-600"
                                    title="Aus Gruppe entfernen"
                                  >
                                    ✕
                                  </button>
                                </div>
                              </div>
                            );
                          })
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </section>

              {/* Unzugeordnete Spieler Pool */}
              <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
                <div className="border-b border-gray-200 px-6 py-4 sm:px-8">
                  <h3 className="text-base font-semibold text-gray-900">
                    Nicht zugeordnete Spieler ({unassignedPlayers.length})
                  </h3>
                  <p className="mt-0.5 text-xs text-gray-500">
                    Wähle für jeden Spieler eine Gruppe aus, um ihn zuzuweisen.
                  </p>
                </div>

                <div className="divide-y divide-gray-100 max-h-96 overflow-y-auto">
                  {unassignedPlayers.map((player) => (
                    <div
                      key={player.id}
                      className="flex items-center justify-between px-6 py-2.5 sm:px-8 hover:bg-gray-50"
                    >
                      <div className="min-w-0">
                        <span className="block text-sm font-medium text-gray-900">
                          {player.displayName}
                        </span>
                        {player.nickname && (
                          <span className="block text-xs text-gray-500">
                            {player.nickname}
                          </span>
                        )}
                      </div>

                      <select
                        defaultValue=""
                        onChange={(e) => {
                          if (e.target.value !== "") {
                            handleAssignPlayerToGroup(
                              player.id,
                              Number(e.target.value),
                            );
                          }
                        }}
                        className="rounded-lg border border-gray-300 bg-white px-2.5 py-1 text-xs font-medium text-gray-700 shadow-sm outline-none focus:border-blue-500"
                      >
                        <option value="" disabled>
                          + Gruppe zuweisen...
                        </option>
                        {manualGroups.map((g, idx) => (
                          <option key={idx} value={idx}>
                            {g.name}
                          </option>
                        ))}
                      </select>
                    </div>
                  ))}

                  {unassignedPlayers.length === 0 && (
                    <div className="px-8 py-8 text-center text-xs text-green-600 font-medium">
                      ✓ Alle ausgewählten Spieler sind einer Gruppe zugeordnet.
                    </div>
                  )}
                </div>
              </section>
            </div>
          )}

          <aside className="space-y-6">
            <section className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
              <div className="border-b border-gray-200 px-6 py-5">
                <h2 className="text-lg font-semibold text-gray-900">
                  {groupMode === "auto"
                    ? "Automatische Einteilung"
                    : "Manuelle Einteilung"}
                </h2>
                <p className="mt-1 text-sm text-gray-500">
                  {groupMode === "auto"
                    ? "Die ausgewählten Spieler werden zufällig auf Gruppen verteilt."
                    : "Matches und Zeitplan für deine manuellen Gruppen erstellen."}
                </p>
              </div>

              <div className="space-y-5 p-6">
                {groupMode === "auto" && (
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
                      Maximal {groupSize} Spieler pro Gruppe. Die Verteilung erfolgt ausgeglichen.
                    </p>
                  </div>
                )}

                <div>
                  <label
                    htmlFor="breakBetweenMatchesMinutes"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Pause zwischen den Spielen in min.
                  </label>

                  <input
                    id="breakBetweenMatchesMinutes"
                    type="number"
                    min={0}
                    value={breakBetweenMatchesMinutes}
                    onChange={(event) =>
                      setBreakBetweenMatchesMinutes(
                        Math.max(0, Number(event.target.value) || 0),
                      )
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                </div>

                <div>
                  <label
                    htmlFor="matchDuration"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Spieldauer in min.
                  </label>

                  <input
                    id="matchDuration"
                    type="number"
                    min={0}
                    value={matchDurationMinutes}
                    onChange={(event) =>
                      setMatchDurationMinutes(
                        Math.max(0, Number(event.target.value) || 0),
                      )
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                </div>

                {groupMode === "auto" ? (
                  <>
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
                          <dt className="text-gray-600">Weiterkommer / Gruppe</dt>
                          <dd className="font-medium text-gray-900">
                            {qualifiersPerGroup}
                          </dd>
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
                  </>
                ) : (
                  <>
                    <div className="rounded-lg border border-blue-100 bg-blue-50 p-4">
                      <p className="text-xs font-medium uppercase tracking-wide text-blue-700">
                        Manuelle Übersicht
                      </p>
                      <dl className="mt-3 space-y-2 text-sm">
                        <div className="flex justify-between gap-4">
                          <dt className="text-gray-600">Gruppen</dt>
                          <dd className="font-medium text-gray-900">
                            {manualGroups.length}
                          </dd>
                        </div>
                        <div className="flex justify-between gap-4">
                          <dt className="text-gray-600">Zugeordnete Spieler</dt>
                          <dd className="font-medium text-gray-900">
                            {assignedPlayerIds.length}
                          </dd>
                        </div>
                        <div className="flex justify-between gap-4">
                          <dt className="text-gray-600">Unzugeordnet</dt>
                          <dd
                            className={`font-medium ${
                              unassignedPlayers.length > 0
                                ? "text-amber-600"
                                : "text-green-600"
                            }`}
                          >
                            {unassignedPlayers.length}
                          </dd>
                        </div>
                      </dl>
                    </div>

                    <button
                      type="button"
                      disabled={
                        generating ||
                        manualGroups.length === 0 ||
                        manualGroups.some((g) => g.playerIds.length < 2)
                      }
                      onClick={() => void handleSaveManual()}
                      className="inline-flex w-full items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500/40 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                      {generating
                        ? "Gruppen werden erstellt..."
                        : "Manuelle Gruppen & Matches erstellen"}
                    </button>
                  </>
                )}

                {groupMode === "auto" && (
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
                )}

                <div>
                  <label
                    htmlFor="koPhaseStartTime"
                    className="block text-sm font-medium text-gray-900"
                  >
                    Startzeit der KO-Phase
                  </label>
                  <input
                    id="koPhaseStartTime"
                    type="time"
                    value={koPhaseStartTime}
                    onChange={(event) =>
                      setKoPhaseStartTime(event.target.value)
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  />
                </div>

                <div>
                  <label
                    htmlFor="knockoutDrawMode"
                    className="block text-sm font-medium text-gray-900"
                  >
                    K.-o.-Auslosungsmethode
                  </label>
                  <select
                    id="knockoutDrawMode"
                    value={knockoutDrawMode}
                    onChange={(event) =>
                      setKnockoutDrawMode(
                        Number(event.target.value) as KnockoutDrawMode,
                      )
                    }
                    className="mt-2 block w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                  >
                    <option value={KnockoutDrawMode.Random}>
                      🎲 Zufällige Auslosung
                    </option>
                    <option value={KnockoutDrawMode.RankedSeeding}>
                      🏆 Beste gegen schlechteste Platzierung
                    </option>
                  </select>
                  <p className="mt-1.5 text-xs text-gray-500 leading-relaxed">
                    {knockoutDrawMode === KnockoutDrawMode.RankedSeeding
                      ? "Setzliste nach Gruppenplatzierung, Siegen & Leg-Differenz. Top-Spieler treffen erst im Finale aufeinander; Freilose für Top-Seeds."
                      : "Qualifizierte Spieler werden zufällig im K.-o.-Baum gegeneinander gelost."}
                  </p>
                </div>

                {knockoutPrepared &&
                  selectedTConfig?.mode ===
                    TournamentMode.GroupStageandKnockout && (
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

            <button
              type="button"
              disabled={generating}
              onClick={() => {
                setTournamentToDelete(tournament);
              }}
              className="inline-flex w-full items-center justify-center gap-2 rounded-lg border border-red-300 bg-red-50 px-4 py-2.5 text-sm font-medium text-red-700 shadow-sm transition hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-red-500/40 disabled:cursor-not-allowed disabled:opacity-60"
            >
              Turnier Löschen
            </button>
          </aside>
        </div>
      </main>

      {tournamentToDelete && (
        <DeleteTournamentModal
          tournament={tournamentToDelete}
          deleting={deleting}
          onCancel={() => setTournamentToDelete(null)}
          onConfirm={handleDelete}
        />
      )}
    </div>
  );
}
