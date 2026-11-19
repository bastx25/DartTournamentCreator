import { useState } from "react";
import type { TournamentDto } from "../../dtos/tournament/TournamentDto";
import type { PublicTournamentDto } from "../../dtos/public/PublicTournamentDto";
import { tournamentStatusLabel } from "../../enums/TournamentStatus";
import { formatDate } from "../../utils/formatDate";

interface DashboardTournamentDetailsProps {
  tournament: TournamentDto | PublicTournamentDto;
  braketsLength: number;
  totalMatches: number;
  activeBraketCount: number;
}

export function DashboardTournamentDetails({
  tournament,
  braketsLength,
  totalMatches,
  activeBraketCount,
}: DashboardTournamentDetailsProps) {
  const [copiedCode, setCopiedCode] = useState(false);
  const [copiedLink, setCopiedLink] = useState(false);

  const handleCopyCode = async () => {
    if (!tournament.code) return;
    await navigator.clipboard.writeText(tournament.code);
    setCopiedCode(true);
    setTimeout(() => setCopiedCode(false), 2000);
  };

  const handleCopyLink = async () => {
    if (!tournament.code) return;
    const shareUrl = `${window.location.origin}/dashboard?code=${tournament.code}`;
    await navigator.clipboard.writeText(shareUrl);
    setCopiedLink(true);
    setTimeout(() => setCopiedLink(false), 2000);
  };

  return (
    <section
      key={tournament.id}
      className="mb-6 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm"
    >
      <div className="border-b border-gray-200 px-6 py-5 sm:px-8">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-2">
              <h2 className="text-xl font-semibold tracking-tight text-gray-900">
                {tournament.name}
              </h2>
              <span className="rounded-full bg-blue-50 px-2.5 py-1 text-xs font-medium text-blue-700">
                {tournamentStatusLabel(tournament.status)}
              </span>
              {tournament.code && (
                <div className="flex items-center gap-1.5 rounded-full border border-gray-200 bg-gray-50 px-3 py-1 text-xs font-mono font-semibold text-gray-700">
                  <span>Code: {tournament.code}</span>
                  <button
                    type="button"
                    onClick={handleCopyCode}
                    className="ml-1 text-gray-500 hover:text-gray-900 transition"
                    title="Code kopieren"
                  >
                    {copiedCode ? "✓" : "📋"}
                  </button>
                </div>
              )}
            </div>
            <p className="mt-2 text-sm text-gray-500">
              Start: {formatDate(tournament.startDate)}
              {tournament.description ? ` · ${tournament.description}` : ""}
            </p>
            {tournament.code && (
              <div className="mt-2">
                <button
                  type="button"
                  onClick={handleCopyLink}
                  className="inline-flex items-center gap-1.5 text-xs font-medium text-blue-600 hover:text-blue-700"
                >
                  <span>🔗</span>
                  {copiedLink ? "Link kopiert!" : "Direktlink zum Turnier kopieren"}
                </button>
              </div>
            )}
          </div>

          <dl className="grid grid-cols-3 gap-4 text-sm sm:min-w-96">
            <div>
              <dt className="text-gray-500">Runden</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {braketsLength}
              </dd>
            </div>
            <div>
              <dt className="text-gray-500">Matches</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {totalMatches}
              </dd>
            </div>
            <div>
              <dt className="text-gray-500">Laufende Runden</dt>
              <dd className="mt-1 text-lg font-semibold text-gray-900">
                {activeBraketCount}
              </dd>
            </div>
          </dl>
        </div>
      </div>
    </section>
  );
}
